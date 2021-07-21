using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static string adventurerReferenceName = "adventurerQuest";
    public static string managerReferenceName = "managerQuest";
    public static string dateFormat = "yyyyMMdd";
    public static QuestManager instance;

    public Dictionary<string, int> AdventurerQuestItemQuantity => adventurerQuest.itemQuantity;

    public Unlockable AdventurerQuestReward => UnlockablesManager.instance.Unlockables[adventurerQuest.UnlockableRewardName];

    [SerializeField] Unlockable lastMushroomUnlockable;

    AdventurerQuest adventurerQuest;
    private ManagerQuest managerQuest;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        FirebaseCommunicator.LoggedIn.AddListener(OnLoggedIn);
        GoldManager.ItemSoldEvent.AddListener(OnItemSold);
    }

    private void Update()
    {

    }

    void OnLoggedIn()
    {
        GetAdventurerQuest();
        GetManagerQuest();
    }

    void GetAdventurerQuest()
    {
        FirebaseCommunicator.instance.GetObject(adventurerReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to get adventurer quest");
                return;
            }
            else if (task.IsCompleted)
            {
                string json = task.Result.GetRawJsonValue();

                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogError("No adventurer quest exists");
                }
                else
                {
                    adventurerQuest = JsonConvert.DeserializeObject<AdventurerQuest>(json);
                    if (adventurerQuest.IsCompleted && !adventurerQuest.IsChecked)
                    {
                        HandleFinishedAdventurerQuest();
                    }
                    else
                    {
                        FirebaseCommunicator.instance.SetupListenForValueChangedEvents(adventurerReferenceName, OnAdventurerQuestChanged);
                    }
                }
            }
        });
    }

    private void OnAdventurerQuestChanged(object sender, ValueChangedEventArgs e)
    {
        string json = e.Snapshot.GetRawJsonValue();
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("No adventurer quest exists");
        }
        else
        {
            adventurerQuest = JsonConvert.DeserializeObject<AdventurerQuest>(json);

            if (adventurerQuest.UnlockableRewardName == null)
            {
                GenerateAdventurerQuestReward();
            }

            if (adventurerQuest.IsCompleted && !adventurerQuest.IsChecked)
            {
                HandleFinishedAdventurerQuest();
            }
        }
    }

    void HandleFinishedAdventurerQuest()
    {
        CheckAdventurerQuest();
        GetAdventurerQuestReward();
    }

    private void GetAdventurerQuestReward()
    {
        Unlockable reward = UnlockablesManager.instance.Unlockables[adventurerQuest.UnlockableRewardName];
        if (reward != null)
        {
            UnlockablesManager.instance.Unlock(reward);
        }
        else
        {
            Debug.LogError("No reward found for adventurer quest");
        }
    }

    public void CheckAdventurerQuest()
    {
        adventurerQuest.CheckQuest();
        SaveAdventurerQuest();
    }

    void GetManagerQuest()
    {
        FirebaseCommunicator.instance.GetObject(managerReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to get manager quest");
                return;
            }
            else if (task.IsCompleted)
            {
                string json = task.Result.GetRawJsonValue();

                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogError("No manager quest exists");
                }
                else
                {
                    managerQuest = JsonConvert.DeserializeObject<ManagerQuest>(json);
                }
            }
        });
    }

    void OnItemSold(Item item, int quantity)
    {
        if (!ExistsAdventurerQuest())
        {
            CreateAdventurerQuest();
            SaveAdventurerQuest();
        }

        if (managerQuest != null && managerQuest.IsChecked && !managerQuest.IsCompleted)
        {
            if (managerQuest.OnSoldItem(item.ItemName, quantity))
            {
                FinishManagerQuest();
            }
            SaveManagerQuest();
        }
    }

    void FinishManagerQuest()
    {
        Debug.Log("Manager Quest Finished");
    }

    void SaveManagerQuest()
    {
        string json = JsonConvert.SerializeObject(managerQuest);
        FirebaseCommunicator.instance.SendObject(json, managerReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to save manager quest");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Manager quest saved");
            }
        });
    }

    public bool ExistsAdventurerQuest()
    {
        return adventurerQuest != null && !adventurerQuest.IsOld();
    }

    void CreateAdventurerQuest()
    {
        List<Item> gatherables = ItemManager.instance.itemsData.Items.FindAll((item) => item.Type == Item.ItemType.Gatherable && item.Unlocked);
        int amountOfItems = Mathf.Min(AdventurerQuest.amountOfItems, gatherables.Count);

        Dictionary<string, int> questItems = new Dictionary<string, int>();

        for (int i = 0; i < amountOfItems; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, gatherables.Count);
            Item item = gatherables[randomIndex];
            while (questItems.ContainsKey(item.ItemName))
            {
                randomIndex = (randomIndex + 1) % gatherables.Count;
                item = gatherables[randomIndex];
            }

            questItems.Add(item.ItemName, UnityEngine.Random.Range(1, AdventurerQuest.maxItemQuantity));
        }

        adventurerQuest = new AdventurerQuest(questItems, null, DateTime.Today.ToString(dateFormat));
        adventurerQuest.GenerateAdventurerQuestReward(lastMushroomUnlockable);
    }

    internal bool AdventurerQuestComplete()
    {
        return adventurerQuest.IsCompleted;
    }

    void SaveAdventurerQuest()
    {
        string json = JsonConvert.SerializeObject(adventurerQuest);
        FirebaseCommunicator.instance.SendObject(json, adventurerReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to save adventurer quest");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Adventurer Quest Saved");
            }
        });
    }

    public void GenerateAdventurerQuestReward()
    {
        adventurerQuest.GenerateAdventurerQuestReward(lastMushroomUnlockable);

        SaveAdventurerQuest();
    }

}

[System.Serializable]
public class AdventurerQuest
{
    public static int amountOfItems = 3;
    public static int maxItemQuantity = 10;
    public static string dateFormat = "yyyyMMdd";
    public Dictionary<string, int> itemQuantity;
    public string UnlockableRewardName { get; private set; }
    public string StartDay { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsChecked { get; private set; }

    public AdventurerQuest(Dictionary<string, int> itemQuantity, string unlockableRewardName, string startDay, bool isCompleted = false, bool isChecked = false)
    {
        this.itemQuantity = itemQuantity;
        UnlockableRewardName = unlockableRewardName;
        StartDay = startDay;
        IsCompleted = isCompleted;
        IsChecked = isChecked;
    }

    public bool CanCompleteQuest(Dictionary<string, int> itemQuantity)
    {
        if (!IsChecked)
        {
            return false;
        }

        bool correctItemQuantity = true;

        foreach (KeyValuePair<string, int> item in this.itemQuantity)
        {
            if (itemQuantity.ContainsKey(item.Key))
            {
                if (item.Value != itemQuantity[item.Key])
                {
                    correctItemQuantity = false;
                    break;
                }
            }
            else
            {
                correctItemQuantity = false;
                break;
            }
        }

        return correctItemQuantity;
    }

    public void CompleteQuest()
    {
        IsCompleted = true;
    }

    internal bool IsOld()
    {
        DateTime start = DateTime.ParseExact(StartDay, dateFormat, null);
        // return true if day is before today
        return (DateTime.Today - start).Days > 0;
    }

    internal void CheckQuest()
    {
        IsChecked = true;
    }

    internal void GenerateAdventurerQuestReward(Unlockable lastMushroomUnlockable)
    {
        Unlockable nextMushroomUnlockable = lastMushroomUnlockable;
        while (!nextMushroomUnlockable.Requirements[0].Unlocked)
        {
            nextMushroomUnlockable = nextMushroomUnlockable.Requirements[0];
        }

        UnlockableRewardName = nextMushroomUnlockable.UnlockableName;
    }
}

[System.Serializable]
internal class ManagerQuest
{
    public static int amountOfItems = 3;
    public static int maxItemQuantity = 10;
    public static string dateFormat = "yyyyMMdd";
    public Dictionary<string, int> Items { get; private set; }
    public string StartDay { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsChecked { get; private set; }

    public ManagerQuest(Dictionary<string, int> items, string startDay, bool isCompleted = false, bool isChecked = false)
    {
        Items = items;
        StartDay = startDay;
        IsCompleted = isCompleted;
        IsChecked = isChecked;
    }

    public bool OnSoldItem(string itemName, int amount)
    {
        if (Items.ContainsKey(itemName))
        {
            Items[itemName] -= amount;
            Items[itemName] = Mathf.Clamp(Items[itemName], 0, maxItemQuantity);
            if (Items[itemName] <= 0)
            {
                if (Items.All((keyValuePair) => keyValuePair.Value == 0))
                {
                    if (!IsCompleted)
                        CompleteQuest();
                }
            }
        }

        return IsCompleted;
    }

    internal void Check()
    {
        IsChecked = true;
    }

    public bool IsOld()
    {
        DateTime today = DateTime.Today;
        DateTime startDay = DateTime.ParseExact(StartDay, dateFormat, null);

        return (today - startDay).Days != 0;
    }

    public void CompleteQuest()
    {
        IsCompleted = true;
        IsChecked = false;
    }
}
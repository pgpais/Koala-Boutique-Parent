using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static string adventurerReferenceName = "adventurerQuest";
    public static string managerReferenceName = "managerQuest";
    public static string dateFormat = "yyyyMMdd";
    public static QuestManager instance;

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
                }
            }
        });
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
        }
    }

    void FinishManagerQuest()
    {
        Debug.Log("Manager Quest Finished");
    }

    bool ExistsAdventurerQuest()
    {
        DateTime startDay = DateTime.ParseExact(managerQuest.StartDay, dateFormat, null);
        DateTime today = DateTime.Today;
        bool isToday = startDay.Date == today.Date;

        return adventurerQuest != null && isToday;
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

        adventurerQuest = new AdventurerQuest(questItems, 100, DateTime.Today.ToString(dateFormat));
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
}

[System.Serializable]
public class AdventurerQuest
{
    public static int amountOfItems = 3;
    public static int maxItemQuantity = 10;

    public Dictionary<string, int> itemQuantity;
    public int GoldReward { get; private set; }
    public string StartDay { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsChecked { get; private set; }

    public AdventurerQuest(Dictionary<string, int> itemQuantity, int goldReward, string startDay, bool isCompleted = false, bool isChecked = false)
    {
        this.itemQuantity = itemQuantity;
        GoldReward = goldReward;
        StartDay = startDay;
        IsCompleted = isCompleted;
        IsChecked = isChecked;
    }

    public bool CanCompleteQuest(Dictionary<string, int> itemQuantity)
    {
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

        return IsChecked && correctItemQuantity;
    }

    public void CompleteQuest()
    {
        IsCompleted = true;
    }
}

[System.Serializable]
internal class ManagerQuest
{
    public Dictionary<string, int> Items { get; private set; }
    public string StartDay { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsChecked { get; private set; }

    public ManagerQuest(Dictionary<string, int> items, string startDay, bool isCompleted, bool isChecked)
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
            if (Items[itemName] <= 0)
            {
                Items.Remove(itemName);
                if (Items.Count == 0)
                {
                    IsCompleted = true;
                }
            }
        }

        return IsCompleted;
    }
}
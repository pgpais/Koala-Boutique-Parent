using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static string adventurerReferenceName = "adventurerQuest";
    public static string managerReferenceName = "managerQuest";
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
}

[System.Serializable]
public class AdventurerQuest
{
    public string Item { get; private set; }
    public int Amount { get; private set; }
    public int GoldReward { get; private set; }
    public string StartDay { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsChecked { get; private set; }

    public AdventurerQuest(string item, int amount, int goldReward, string startDay, bool isCompleted = false, bool isChecked = false)
    {
        Item = item;
        Amount = amount;
        GoldReward = goldReward;
        StartDay = startDay;
        IsCompleted = isCompleted;
        IsChecked = isChecked;
    }

    public bool CanCompleteQuest(string itemName, int amount)
    {
        return IsChecked && itemName == Item && amount == Amount;
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
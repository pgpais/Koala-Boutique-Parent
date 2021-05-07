using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    public static UnityEvent<Item, int> NewItemAdded = new UnityEvent<Item, int>();

    [field: SerializeField] public ItemsList itemsData { get; private set; }
    [field: SerializeField] public Dictionary<string, int> itemQuantity { get; private set; }


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

        itemQuantity = new Dictionary<string, int>();

        itemsData.InitializeEvents();
    }

    private void Start()
    {
        GetCloudItems();
    }

    public bool HasEnoughItem(string itemName, int amount)
    {
        return itemQuantity[itemName] >= amount;
    }

    public void AddItem(string itemName, int amount)
    {
        if (itemsData.ContainsByName(itemName))
        {
            Item item = itemsData.GetItemByName(itemName);

            if (itemQuantity.ContainsKey(itemName))
            {
                itemQuantity[itemName] += amount;
                item.ItemUpdated.Invoke(itemQuantity[itemName]);
                UpdateCloudItem(itemName, itemQuantity[itemName]);
            }
            else
            {
                itemQuantity.Add(itemName, amount);
                NewItemAdded.Invoke(item, amount);
                UpdateCloudItem(itemName, amount);
            }
        }
    }

    private void UpdateCloudItem(string itemName, int amount)
    {
        var dictionary = new Dictionary<string, object>();
        dictionary[itemName] = amount;
        FirebaseCommunicator.instance.UpdateObject(dictionary, "items", (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey updated items");
            }
        });
    }

    private void GetCloudItems()
    {
        FirebaseCommunicator.instance.GetObject("items", (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey got items");
                Dictionary<string, object> dictionary = task.Result.Value as Dictionary<string, object>;
                foreach (var key in dictionary.Keys)
                {
                    Debug.Log($"Found item {key} with amount {dictionary[key]}");
                    itemQuantity.Add(key, Convert.ToInt32(dictionary[key]));
                    NewItemAdded.Invoke(itemsData.GetItemByName(key), Convert.ToInt32(dictionary[key]));
                }
            }
        });
    }
}

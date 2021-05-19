using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System;
using Firebase.Database;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    public static UnityEvent<Item, int> NewItemAdded = new UnityEvent<Item, int>();
    public static UnityEvent<Item, int> ItemUpdated = new UnityEvent<Item, int>();
    public static UnityEvent<Item> ItemRemoved = new UnityEvent<Item>();

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
        FirebaseCommunicator.LoggedIn.AddListener(OnLoggedIn);
    }

    private void OnLoggedIn()
    {
        // GetCloudItems();
        SetupCloudListeners();
    }

    public bool HasEnoughItem(string itemName, int amount)
    {
        return itemQuantity[itemName] >= amount;
    }

    public void AddItem(string itemName, int amount, bool syncCloud)
    {
        if (itemsData.ContainsByName(itemName))
        {
            Item item = itemsData.GetItemByName(itemName);

            if (itemQuantity.ContainsKey(itemName))
            {
                itemQuantity[itemName] += amount;
                ItemUpdated.Invoke(item, itemQuantity[itemName]);

                if (syncCloud)
                    UpdateCloudItem(itemName, itemQuantity[itemName]);
            }
            else
            {
                itemQuantity.Add(itemName, amount);
                NewItemAdded.Invoke(item, amount);

                if (syncCloud)
                    UpdateCloudItem(itemName, amount);
            }
        }
    }

    public void RemoveItem(string itemName, int amount)
    {
        if (itemQuantity[itemName] > amount)
        {
            itemQuantity[itemName] -= amount;
            ItemUpdated.Invoke(itemsData.GetItemByName(itemName), amount);
            UpdateCloudItem(itemName, itemQuantity[itemName]);
        }
        else if (itemQuantity[itemName] == amount)
        {
            itemQuantity.Remove(itemName);
            itemsData.GetItemByName(itemName).ItemRemoved.Invoke();
            RemoveCloudItem(itemName);
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

    private void RemoveCloudItem(string itemname)
    {
        FirebaseCommunicator.instance.RemoveObject("items", itemname, (task, obj) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey removed " + itemname + " from items");
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
                    AddItem(key, Convert.ToInt32(dictionary[key]), false);
                }
            }
        });
    }

    private void SetupCloudListeners()
    {
        FirebaseCommunicator.instance.SetupListenForChildChangedEvents(new string[] { "items", FirebaseCommunicator.instance.FamilyId.ToString() }, OnGlobalInventoryItemChanged);

        FirebaseCommunicator.instance.SetupListenForChildAddedEvents(new string[] { "items", FirebaseCommunicator.instance.FamilyId.ToString() }, OnGlobalInventoryItemAdded);

        FirebaseCommunicator.instance.SetupListenForChildRemovedEvents(new string[] { "items", FirebaseCommunicator.instance.FamilyId.ToString() }, OnGlobalInventoryItemRemoved);
    }

    private void OnGlobalInventoryItemChanged(object sender, ChildChangedEventArgs e)
    {
        Debug.Log("CLOUD: items were updated");

        itemQuantity[e.Snapshot.Key] = Convert.ToInt32(e.Snapshot.Value);

        ItemUpdated.Invoke(itemsData.GetItemByName(e.Snapshot.Key), itemQuantity[e.Snapshot.Key]);
    }

    private void OnGlobalInventoryItemAdded(object sender, ChildChangedEventArgs e)
    {
        Debug.Log("CLOUD: New item added");

        itemQuantity.Add(e.Snapshot.Key, Convert.ToInt32(e.Snapshot.Value));

        NewItemAdded.Invoke(itemsData.GetItemByName(e.Snapshot.Key), itemQuantity[e.Snapshot.Key]);
    }

    private void OnGlobalInventoryItemRemoved(object sender, ChildChangedEventArgs e)
    {
        Debug.Log("CLOUD: item removed");

        itemQuantity.Remove(e.Snapshot.Key);

        itemsData.GetItemByName(e.Snapshot.Key).ItemRemoved.Invoke();
    }
}

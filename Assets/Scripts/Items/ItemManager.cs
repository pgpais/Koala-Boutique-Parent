using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

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
            }
            else
            {
                itemQuantity.Add(itemName, amount);
                NewItemAdded.Invoke(item, amount);
            }
        }
    }
}

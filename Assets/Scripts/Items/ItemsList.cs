using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsList", menuName = "Ye Olde Shop/ItemsList", order = 0)]
public class ItemsList : ScriptableObject
{
    [SerializeField] List<Item> items;

    public bool ContainsByName(string itemName)
    {
        return items.Any(item => item.ItemName.Equals(itemName));
    }

    public Item GetItemByName(string itemName)
    {
        return items.Find(item => item.ItemName.Equals(itemName));
    }

    internal void InitializeEvents()
    {
        foreach (var item in items)
        {
            item.InitializeEvent();
        }
    }
}
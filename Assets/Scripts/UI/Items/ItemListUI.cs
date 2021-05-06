using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemListUI : MonoBehaviour
{
    [SerializeField] ItemUI itemUIPrefab;

    private void Start()
    {
        ItemManager.NewItemAdded.AddListener(NewItemAdded);
    }

    public void Init(List<Item> items, Dictionary<string, int> itemsQuantity)
    {
        foreach (var item in items)
        {
            Instantiate(itemUIPrefab, transform).Init(item, itemsQuantity[item.ItemName]);
        }
    }

    public void NewItemAdded(Item item, int itemQuantity)
    {
        Instantiate(itemUIPrefab, transform).Init(item, itemQuantity);
    }
}

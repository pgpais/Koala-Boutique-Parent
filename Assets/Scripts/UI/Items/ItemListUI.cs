using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemListUI : MonoBehaviour
{
    [SerializeField] ItemUI itemUIPrefab;
    [SerializeField] Transform availableItems;
    [SerializeField] Transform unavailableItems;

    Dictionary<string, ItemUI> availableItemUIs;
    Dictionary<string, ItemUI> unavailableItemUIs;

    private void Awake()
    {

    }

    private void Start()
    {
    }

    private void OnEnable()
    {
        availableItemUIs = new Dictionary<string, ItemUI>();
        unavailableItemUIs = new Dictionary<string, ItemUI>();
        ItemManager.NewItemAdded.AddListener(NewItemAdded);
        ItemManager.ItemRemoved.AddListener(ItemRemoved);
        Init(ItemManager.instance.itemQuantity);
    }

    private void OnDisable()
    {
        ItemManager.NewItemAdded.RemoveListener(NewItemAdded);
    }

    public void Init(Dictionary<string, int> itemsQuantity)
    {

        foreach (var item in ItemManager.instance.itemsData.Items)
        {
            ItemUI itemUI = Instantiate(itemUIPrefab);
            if (itemsQuantity.ContainsKey(item.ItemName))
            {
                // add item to available items UI
                itemUI.transform.SetParent(this.availableItems.transform, false);
                availableItemUIs.Add(item.ItemName, itemUI);
                itemUI.Init(item, itemsQuantity[item.ItemName]);
            }
            else
            {
                // add item to unavailable items UI
                itemUI.transform.SetParent(this.unavailableItems.transform, false);
                unavailableItemUIs.Add(item.ItemName, itemUI);
                itemUI.Init(item);
            }
        }

        // foreach (var itemName in availableItems)
        // {
        //     var itemdata = ItemManager.instance.itemsData;
        //     if (itemdata == null)
        //         Debug.LogError("NULL ITEMDATA");
        //     var item = itemdata.GetItemByName(itemName);
        //     if (item == null)
        //         Debug.LogError("NULL ITEM");
        //     if (itemUIPrefab == null)
        //         Debug.LogError("NULL PREFAB");
        //     Instantiate(itemUIPrefab, transform).Init(item, itemsQuantity[itemName]);
        // }
    }

    public void NewItemAdded(Item item, int itemQuantity)
    {
        MakeAvailable(item, itemQuantity);
    }

    private void MakeAvailable(Item item, int itemQuantity)
    {
        // remove item from unavailable
        var itemUI = unavailableItemUIs[item.ItemName];
        unavailableItemUIs.Remove(item.ItemName);

        // add to available
        itemUI.transform.SetParent(availableItems);
        itemUI.MakeAvailable(itemQuantity);
        availableItemUIs.Add(item.ItemName, itemUI);
    }

    public void ItemRemoved(Item item)
    {
        MakeUnavailable(item);
    }
    private void MakeUnavailable(Item item)
    {
        var itemUI = availableItemUIs[item.ItemName];
        availableItemUIs.Remove(item.ItemName);

        itemUI.transform.SetParent(unavailableItems);
        itemUI.MakeUnavailable();
        unavailableItemUIs.Add(item.ItemName, itemUI);
    }
}

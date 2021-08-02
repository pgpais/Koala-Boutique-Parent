using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemListUI : MonoBehaviour
{
    [SerializeField] ItemUI itemUIPrefab;
    [SerializeField] Transform availableItems;
    [SerializeField] Transform processedItems;

    Dictionary<string, ItemUI> availableItemUIs;
    Dictionary<string, ItemUI> unavailableItemUIs;

    private void Awake()
    {
        availableItemUIs = new Dictionary<string, ItemUI>();
        unavailableItemUIs = new Dictionary<string, ItemUI>();

        if (ItemManager.instance.GotItems)
        {
            Init(ItemManager.instance.itemQuantity);
        }
        else
        {
            ItemManager.OnGotItems.AddListener(() => Init(ItemManager.instance.itemQuantity));
        }
    }

    private void Start()
    {
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        // ItemManager.NewItemAdded.RemoveListener(NewItemAdded);
        // ItemManager.ItemRemoved.RemoveListener(ItemRemoved);
        // foreach (Transform item in availableItems)
        // {
        //     Destroy(item.gameObject);
        // }

        // foreach (Transform item in processedItems)
        // {
        //     Destroy(item.gameObject);
        // }
    }

    public void Init(Dictionary<string, int> itemsQuantity)
    {
        ItemManager.NewItemAdded.AddListener(NewItemAdded);
        ItemManager.ItemRemoved.AddListener(ItemRemoved);
        StartCoroutine(SpawnItemUIs(itemsQuantity));

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

    private IEnumerator SpawnItemUIs(Dictionary<string, int> itemsQuantity)
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

                if (item.Type == Item.ItemType.Processed)
                {
                    itemUI.transform.SetParent(this.processedItems.transform, false);
                }
                else
                {
                    itemUI.transform.SetParent(this.availableItems.transform, false);
                }

                if (item.ItemName == SecretDoorManager.instance.DoorKey.ItemName || item.ItemName == SecretDoorManager.instance.DoorKey.ProcessResult.ItemName)
                {
                    itemUI.gameObject.SetActive(true);
                }
            }
            else
            {
                // add item to unavailable items UI
                itemUI.transform.SetParent(this.processedItems.transform, false);
                unavailableItemUIs.Add(item.ItemName, itemUI);
                itemUI.Init(item);

                if (item.ItemName == SecretDoorManager.instance.DoorKey.ItemName || item.ItemName == SecretDoorManager.instance.DoorKey.ProcessResult.ItemName)
                {
                    itemUI.gameObject.SetActive(false);
                }

                if (item.Type == Item.ItemType.Processed)
                {
                    itemUI.transform.SetParent(this.processedItems.transform, false);
                }
                else
                {
                    itemUI.transform.SetParent(this.availableItems.transform, false);
                }
            }
            yield return null;
        }

        SortUI();
    }

    public void NewItemAdded(Item item, int itemQuantity)
    {
        MakeAvailable(item, itemQuantity);
        SortUI();
    }

    private void MakeAvailable(Item item, int itemQuantity)
    {
        // remove item from unavailable
        var itemUI = unavailableItemUIs[item.ItemName];
        unavailableItemUIs.Remove(item.ItemName);

        // add to available
        itemUI.MakeAvailable(itemQuantity);
        availableItemUIs.Add(item.ItemName, itemUI);
        if (item.ItemName == "Encrypted Key" || item.ItemName == "Decrypted Key")
        {
            unavailableItemUIs[item.ItemName].gameObject.SetActive(true);
        }
    }

    public void ItemRemoved(Item item)
    {
        MakeUnavailable(item);
        SortUI();
    }
    private void MakeUnavailable(Item item)
    {
        var itemUI = availableItemUIs[item.ItemName];
        availableItemUIs.Remove(item.ItemName);

        itemUI.MakeUnavailable();
        unavailableItemUIs.Add(item.ItemName, itemUI);
        if (item.ItemName == "Encrypted Key" || item.ItemName == "Decrypted Key")
        {
            unavailableItemUIs[item.ItemName].gameObject.SetActive(false);
        }
    }

    void SortUI()
    {
        SortRaw();
        SortProcessed();
    }

    void SortRaw()
    {
        List<ItemUI> rawItems = availableItemUIs.Values.Where(itemUI => itemUI.Item.Type != Item.ItemType.Processed).ToList();

        rawItems.Sort((item1, item2) => item1.CompareTo(item2));

        for (int i = 0; i < rawItems.Count; i++)
        {
            rawItems[i].transform.SetSiblingIndex(i + 2); // +2 because of the title and the separator
        }
    }

    void SortProcessed()
    {
        List<ItemUI> processedItems = availableItemUIs.Values.Where(itemUI => itemUI.Item.Type == Item.ItemType.Processed).ToList();

        processedItems.Sort((item1, item2) => item1.CompareTo(item2));

        for (int i = 0; i < processedItems.Count; i++)
        {
            processedItems[i].transform.SetSiblingIndex(i + 2); // +2 because of the title and the separator
        }
    }
}

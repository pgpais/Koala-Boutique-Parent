using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemListUI : MonoBehaviour
{
    [SerializeField] ItemUI itemUIPrefab;

    private void Awake()
    {

    }

    private void Start()
    {
        ItemManager.NewItemAdded.AddListener(NewItemAdded);
    }

    private void OnEnable()
    {
        Init(ItemManager.instance.itemQuantity);
    }

    public void Init(Dictionary<string, int> itemsQuantity)
    {
        var items = itemsQuantity.Keys;
        foreach (var itemName in items)
        {
            Debug.Log("Instantiating item");
            var item = ItemManager.instance.itemsData.GetItemByName(itemName);
            Instantiate(itemUIPrefab, transform).Init(item, itemsQuantity[itemName]);
        }
    }

    public void NewItemAdded(Item item, int itemQuantity)
    {
        Debug.Log("Instantiating item");
        Instantiate(itemUIPrefab, transform).Init(item, itemQuantity);
    }
}

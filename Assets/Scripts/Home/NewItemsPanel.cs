using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewItemsPanel : MonoBehaviour
{
    [SerializeField] int maxItemQuantity;
    [Space]
    [SerializeField] LayoutGroup newItemsLayoutGroup;
    [SerializeField] TMP_Text diseasedGoldLossText;
    [SerializeField] Image diseasedItemImage;
    [SerializeField] SmallItemUI smallItemUI;


    int diseasedGoldLoss;
    Item diseasedItem;
    Dictionary<Item, int> newItems;

    public int Init(int diseasedGoldLoss, Item diseasedItem, Dictionary<Item, int> newItems)
    {
        this.diseasedGoldLoss = diseasedGoldLoss;
        this.diseasedItem = diseasedItem;
        this.newItems = newItems;

        UpdateUI();

        return maxItemQuantity;
    }

    void UpdateUI()
    {
        if (diseasedGoldLoss < 0 && diseasedItem != null)
        {
            diseasedGoldLossText.text = diseasedGoldLoss.ToString();
            diseasedItemImage.sprite = diseasedItem.ItemSprite;
        }

        foreach (Transform child in newItemsLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        int spawnedItems = 0;
        List<Item> addedItems = new List<Item>();
        foreach (KeyValuePair<Item, int> newItem in newItems)
        {
            SmallItemUI newSmallItemUI = Instantiate(smallItemUI);
            newSmallItemUI.transform.SetParent(newItemsLayoutGroup.transform);
            newSmallItemUI.InitUI(newItem.Key, newItem.Value);

            addedItems.Add(newItem.Key);

            spawnedItems++;
            if (spawnedItems >= maxItemQuantity)
            {
                break;
            }
        }

        foreach (var item in addedItems)
        {
            newItems.Remove(item);
        }
    }

}

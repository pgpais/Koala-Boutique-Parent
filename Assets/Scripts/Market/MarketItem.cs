using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    public bool AvailableItem { get; private set; }


    public string ItemName => itemName.text;
    [SerializeField] TMP_Text itemName;
    public TMP_Text itemValue;
    public Image itemImage;

    [SerializeField] Image fadeImage;

    [SerializeField] Button bargainButton;
    [SerializeField] Button sellButton;

    private void Awake()
    {
        bargainButton.onClick.AddListener(() =>
        {
            if (!bargainButton.interactable)
            {
                return;
            }

            int itemQuantity = ItemManager.instance.itemQuantity[ItemName];
            ItemManager.instance.ShowSellGameMenu(ItemName, 0, itemQuantity);
        });

        sellButton.onClick.AddListener(() =>
        {
            if (!sellButton.interactable)
            {
                return;
            }

            int itemQuantity = ItemManager.instance.itemQuantity[ItemName];
            ItemManager.instance.ShowSellMenu(ItemName, 0, itemQuantity);
        });
    }

    public void Init(string itemName, int itemValue, Sprite itemImage)
    {
        this.itemName.text = itemName;
        this.itemValue.text = itemValue.ToString();

        if (itemImage != null)
            this.itemImage.sprite = itemImage;

        AvailableItem = ItemManager.instance.itemQuantity.ContainsKey(itemName);
        bargainButton.interactable = AvailableItem;
        sellButton.interactable = AvailableItem;
        fadeImage.enabled = !AvailableItem;

        bargainButton.targetGraphic.enabled = AvailableItem;
        sellButton.targetGraphic.enabled = AvailableItem;

        MarketPrices.GotMarketPrices.AddListener(OnPricesUpdated);
    }

    public void UpdateUI(string itemName, int itemValue, Sprite itemImage)
    {
        this.itemName.text = itemName;
        this.itemValue.text = itemValue.ToString();

        if (itemImage != null)
            this.itemImage.sprite = itemImage;

        AvailableItem = ItemManager.instance.itemQuantity.ContainsKey(itemName);
        bargainButton.interactable = AvailableItem;
        sellButton.interactable = AvailableItem;
        fadeImage.enabled = !AvailableItem;

        bargainButton.targetGraphic.enabled = AvailableItem;
        sellButton.targetGraphic.enabled = AvailableItem;
    }

    public void UpdateValue(int itemValue)
    {
        this.itemValue.text = itemValue.ToString();
    }

    void OnPricesUpdated()
    {
        int modifier = MarketPrices.instance.GetCostModifierForItem(ItemName);
        int itemValue = ItemManager.instance.itemsData.GetItemByName(ItemName).GoldValue;
        UpdateValue(itemValue + modifier);
    }
}

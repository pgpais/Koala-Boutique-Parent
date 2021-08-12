using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Sirenix.OdinInspector;

public class MarketItem : SerializedMonoBehaviour, IComparable<MarketItem>
{
    public bool AvailableItem { get; private set; }


    public string ItemNameKey => item.ItemNameKey;
    [SerializeField] TMP_Text itemName;
    [SerializeField] GameObject quantityLabel;
    public TMP_Text itemQuantity;
    public TMP_Text itemValue;
    public Image itemImage;

    [SerializeField] Image fadeImage;

    [SerializeField] Button bargainButton;
    [SerializeField] Image bargainImage;
    [SerializeField] Dictionary<Language, Sprite> bargainImageSprites = new Dictionary<Language, Sprite>();
    [SerializeField] Button sellButton;
    [SerializeField] Image sellImage;
    [SerializeField] Dictionary<Language, Sprite> sellImageSprites = new Dictionary<Language, Sprite>();

    Item item;

    private void Awake()
    {
        bargainImage.sprite = bargainImageSprites[Localisation.currentLanguage];
        sellImage.sprite = sellImageSprites[Localisation.currentLanguage];

        bargainButton.onClick.AddListener(() =>
        {
            if (!bargainButton.interactable)
            {
                return;
            }

            int itemQuantity = ItemManager.instance.itemQuantity[ItemNameKey];
            ItemManager.instance.ShowSellGameMenu(ItemNameKey, 0, itemQuantity);
        });

        sellButton.onClick.AddListener(() =>
        {
            if (!sellButton.interactable)
            {
                return;
            }

            int itemQuantity = ItemManager.instance.itemQuantity[ItemNameKey];
            ItemManager.instance.ShowSellMenu(ItemNameKey, 0, itemQuantity);
        });
    }

    public void Init(string itemNameKey, int itemValue, Sprite itemImage)
    {
        item = ItemManager.instance.itemsData.GetItemByName(itemNameKey);


        this.itemName.text = item.ItemName;
        this.itemValue.text = itemValue.ToString();

        if (itemImage != null)
            this.itemImage.sprite = itemImage;

        AvailableItem = ItemManager.instance.itemQuantity.ContainsKey(itemNameKey);

        if (AvailableItem)
        {
            this.itemQuantity.text = ItemManager.instance.itemQuantity[itemNameKey].ToString();
        }

        quantityLabel.SetActive(AvailableItem);

        bargainButton.interactable = AvailableItem;
        sellButton.interactable = AvailableItem;
        fadeImage.enabled = !AvailableItem;

        bargainButton.targetGraphic.enabled = AvailableItem;
        sellButton.targetGraphic.enabled = AvailableItem;

        MarketPrices.GotMarketPrices.AddListener(OnPricesUpdated);
    }

    public void UpdateUI(string itemNameKey, int itemValue, Sprite itemImage)
    {
        item = ItemManager.instance.itemsData.GetItemByName(itemNameKey);

        this.itemName.text = itemNameKey;

        this.itemValue.text = itemValue.ToString();

        if (itemImage != null)
            this.itemImage.sprite = itemImage;

        AvailableItem = ItemManager.instance.itemQuantity.ContainsKey(itemNameKey);

        if (AvailableItem)
        {
            this.itemQuantity.text = ItemManager.instance.itemQuantity[itemNameKey].ToString();
        }

        quantityLabel.SetActive(AvailableItem);

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
        int modifier = MarketPrices.instance.GetCostModifierForItem(ItemNameKey);
        int itemValue = ItemManager.instance.itemsData.GetItemByName(ItemNameKey).GoldValue;
        UpdateValue(itemValue + modifier);
    }

    public int CompareTo(MarketItem other)
    {
        if (this.AvailableItem && !other.AvailableItem)
        {
            return -1;
        }
        else if (!this.AvailableItem && other.AvailableItem)
        {
            return 1;
        }
        else
        {
            Item item1 = ItemManager.instance.itemsData.GetItemByName(ItemNameKey);
            Item item2 = ItemManager.instance.itemsData.GetItemByName(other.ItemNameKey);
            return item1.CompareTo(item2);
        }
    }

    internal void UpdateAmount(int amount)
    {
        itemQuantity.text = amount.ToString();
    }
}

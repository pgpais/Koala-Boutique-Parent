using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] TMP_Text itemNameText;
    // [SerializeField] TMP_Text itemDescriptionText;
    [SerializeField] TMP_Text itemQuantityText;
    [SerializeField] GameObject itemQuantityLabel;
    // [SerializeField] TMP_Text itemValueText;
    [SerializeField] Image itemImage;
    [SerializeField] Button startProcessingItemButton;
    [SerializeField] Toggle processOneItemToggle;
    [SerializeField] Toggle processTenItemToggle;
    [SerializeField] Toggle processAllItemToggle;
    // [SerializeField] Button sellItemButton;

    private bool available = false;

    private Item item;

    private void Awake()
    {
        MarketPrices.GotMarketPrices.AddListener(UpdateItemPrices);
    }

    public void Init(Item item, int itemQuantity)
    {
        this.item = item;

        itemNameText.text = item.ItemName;
        // itemDescriptionText.text = item.Description;
        // this.itemValueText.transform.parent.gameObject.SetActive(false);

        available = true;
        MakeAvailable(itemQuantity);

        if (MarketPrices.instance.hasPrices)
        {
            UpdateItemPrices();
        }

        if (item.ItemSprite != null)
        {
            itemImage.sprite = item.ItemSprite;
        }
    }

    public void Init(Item item)
    {
        this.item = item;

        itemNameText.text = item.ItemName;
        // itemDescriptionText.text = item.Description;
        // this.itemValueText.transform.parent.gameObject.SetActive(false);

        available = false;
        MakeUnavailable();

        if (MarketPrices.instance.hasPrices)
        {
            UpdateItemPrices();
        }

        if (item.ItemSprite != null)
        {
            itemImage.sprite = item.ItemSprite;
        }
    }

    private void UpdateUI(Item item, int quantity)
    {
        if (item.ItemName == itemNameText.text)
            itemQuantityText.text = quantity.ToString();
    }

    private void OnEnable()
    {
        MarketPrices.GotMarketPrices.AddListener(UpdateItemPrices);
    }

    private void OnDisable()
    {
        MarketPrices.GotMarketPrices.RemoveListener(UpdateItemPrices);
        // Destroy(gameObject);
    }

    private void ShowStartProcessScreen()
    {

        ProcessingManager.instance.ShowNewProcessMenu(itemNameText.text, 0, ItemManager.instance.itemQuantity[itemNameText.text]);
    }

    private void ShowSellScreen()
    {
        ItemManager.instance.ShowSellGameMenu(item.ItemName, 0, ItemManager.instance.itemQuantity[item.ItemName]);
    }

    void UpdateItemPrices()
    {
        // this.itemValueText.transform.parent.gameObject.SetActive(true);

        int itemValue = item.GoldValue;
        itemValue += MarketPrices.instance.GetCostModifierForItem(item.ItemName);
        // this.itemValueText.text = itemValue.ToString();
    }

    public void MakeUnavailable()
    {
        itemQuantityText.gameObject.SetActive(false);
        itemQuantityLabel.gameObject.SetActive(false);
        // sellItemButton.gameObject.SetActive(false);
        startProcessingItemButton.gameObject.SetActive(false);
        processOneItemToggle.gameObject.SetActive(false);
        processTenItemToggle.gameObject.SetActive(false);
        processAllItemToggle.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void MakeAvailable(int itemQuantity)
    {
        gameObject.SetActive(true);
        itemQuantityText.gameObject.SetActive(true);
        itemQuantityLabel.gameObject.SetActive(true);
        // sellItemButton.gameObject.SetActive(true);
        if (item.ProcessResult != null)
        {
            startProcessingItemButton.gameObject.SetActive(true);
            processOneItemToggle.gameObject.SetActive(true);
            processTenItemToggle.gameObject.SetActive(true);
            processAllItemToggle.gameObject.SetActive(true);
        }
        else
        {
            startProcessingItemButton.gameObject.SetActive(false);
            processOneItemToggle.gameObject.SetActive(false);
            processTenItemToggle.gameObject.SetActive(false);
            processAllItemToggle.gameObject.SetActive(false);
        }

        itemQuantityText.text = itemQuantity.ToString();

        // TODO: #13 Move item event listeners to parent UI script
        ItemManager.ItemUpdated.AddListener(UpdateUI);
        startProcessingItemButton.onClick.AddListener(StartProcessing);
        // sellItemButton.onClick.AddListener(ShowSellScreen);

        bool canItemBeProcessed = !(item.Type == Item.ItemType.Processed || item.Type == Item.ItemType.Valuable);
        if (!canItemBeProcessed)
            startProcessingItemButton.transform.parent.gameObject.SetActive(false);
    }

    private void StartProcessing()
    {
        int amount = 0;

        if (processOneItemToggle.isOn)
        {
            amount = 1;
        }
        else if (processTenItemToggle.isOn)
        {
            amount = 10;
        }
        else if (processAllItemToggle.isOn)
        {
            amount = ItemManager.instance.itemQuantity[item.ItemName];
        }

        ProcessingManager.instance.StartProcessing(item.ItemName, amount);
    }
}

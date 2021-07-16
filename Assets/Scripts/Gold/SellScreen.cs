using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellScreen : MonoBehaviour
{

    [SerializeField] Image itemImage;
    [SerializeField] TMPro.TMP_Text itemValueText;
    [SerializeField] TMPro.TMP_Text sellAmountText;
    [SerializeField] Button increaseButton;
    [SerializeField] Button decreaseButton;
    [SerializeField] Button sellButton;
    [SerializeField] TMPro.TMP_Text sellButtonText;
    [SerializeField] Button backButton;
    // [SerializeField] TMPro.TMP_Text resultText;

    private string itemName;
    private Item item;
    private int sellAmount;
    private int maxAmount;
    private int minAmount;

    // Start is called before the first frame update
    void Start()
    {
        increaseButton.onClick.AddListener(() =>
        {
            sellAmount++;
            sellAmount = Mathf.Clamp(sellAmount, minAmount, maxAmount);
            UpdateUI();
        });
        decreaseButton.onClick.AddListener(() =>
        {
            sellAmount--;
            sellAmount = Mathf.Clamp(sellAmount, minAmount, maxAmount);
            UpdateUI();
        });
        sellButton.onClick.AddListener(SellItem);
        backButton.onClick.AddListener(CloseMenu);
    }

    public void Init(string itemName, int minAmount, int maxAmount)
    {
        item = ItemManager.instance.itemsData.GetItemByName(itemName);

        this.itemName = itemName;
        this.maxAmount = maxAmount;
        this.minAmount = minAmount;
        sellAmount = 0;

        gameObject.SetActive(true);
        transform.parent.gameObject.SetActive(true);

        UpdateUI();
    }

    void SellItem()
    {
        ItemManager.instance.SellItem(itemName, sellAmount);
        CloseMenu();
    }

    void UpdateUI()
    {
        itemImage.sprite = item.ItemSprite;
        sellAmountText.text = sellAmount.ToString();

        int itemValue = item.GoldValue + MarketPrices.instance.GetCostModifierForItem(item.ItemName);
        itemValueText.text = itemValue.ToString();
        sellButtonText.text = (itemValue * sellAmount).ToString();
    }

    // void OnSliderUpdated(float value)
    // {
    //     int currentAmount = (int)value;
    //     UpdateResultText(currentAmount);
    //     if (currentAmount == 0)
    //     {
    //         sellButton.interactable = false;
    //     }
    //     else
    //     {
    //         sellButton.interactable = true;

    //     }
    // }

    // void UpdateResultText(int currentAmount)
    // {
    //     resultText.text = $"You will sell {currentAmount} of {itemName} for {(item.GoldValue * currentAmount) + MarketPrices.instance.GetCostModifierForItem(itemName)} gold.";
    // }

    void CloseMenu()
    {
        gameObject.SetActive(false);
        transform.parent.gameObject.SetActive(false);
    }
}

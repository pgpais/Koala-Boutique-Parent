using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellScreen : MonoBehaviour
{

    [SerializeField] Image itemImage;
    [SerializeField] TMPro.TMP_Text sellAmountText;
    [SerializeField] Button increaseButton;
    [SerializeField] Button decreaseButton;
    [SerializeField] Button sellButton;
    [SerializeField] Button backButton;
    // [SerializeField] TMPro.TMP_Text resultText;

    private string itemName;
    private Item item;
    private int sellAmount;

    // Start is called before the first frame update
    void Start()
    {
        increaseButton.onClick.AddListener(() =>
        {
            sellAmount++;
            UpdateUI();
        });
        decreaseButton.onClick.AddListener(() =>
        {
            sellAmount--;
            UpdateUI();
        });
        sellButton.onClick.AddListener(SellItem);
        backButton.onClick.AddListener(CloseMenu);
    }

    public void Init(string itemName, int minAmount, int maxAmount)
    {
        item = ItemManager.instance.itemsData.GetItemByName(itemName);

        this.itemName = itemName;
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

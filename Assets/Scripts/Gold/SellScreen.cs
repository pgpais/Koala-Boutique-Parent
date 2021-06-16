using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellScreen : MonoBehaviour
{

    [SerializeField] TMPro.TMP_Text maxAmountText;
    [SerializeField] TMPro.TMP_Text minAmountText;
    [SerializeField] Slider amountSlider;
    [SerializeField] Button sellButton;
    [SerializeField] Button backButton;
    [SerializeField] TMPro.TMP_Text resultText;

    private string itemName;
    private Item item;

    // Start is called before the first frame update
    void Start()
    {
        amountSlider.onValueChanged.AddListener(OnSliderUpdated);
        sellButton.onClick.AddListener(SellItem);
        backButton.onClick.AddListener(CloseMenu);
    }

    public void Init(string itemName, int minAmount, int maxAmount)
    {
        item = ItemManager.instance.itemsData.GetItemByName(itemName);

        this.itemName = itemName;
        this.amountSlider.minValue = minAmount;
        minAmountText.text = minAmount.ToString();
        this.amountSlider.maxValue = maxAmount;
        maxAmountText.text = maxAmount.ToString();

        UpdateResultText(minAmount);
        amountSlider.value = minAmount;

        gameObject.SetActive(true);
        transform.parent.gameObject.SetActive(true);
    }

    void SellItem()
    {
        ItemManager.instance.SellItem(itemName, (int)amountSlider.value);
        CloseMenu();
    }

    void OnSliderUpdated(float value)
    {
        int currentAmount = (int)value;
        UpdateResultText(currentAmount);
        if (currentAmount == 0)
        {
            sellButton.interactable = false;
        }
        else
        {
            sellButton.interactable = true;

        }
    }

    void UpdateResultText(int currentAmount)
    {
        resultText.text = $"You will sell {currentAmount} of {itemName} for {(item.GoldValue * currentAmount) + MarketPrices.instance.GetCostModifierForItem(itemName)} gold.";
    }

    void CloseMenu()
    {
        gameObject.SetActive(false);
        transform.parent.gameObject.SetActive(false);
    }
}

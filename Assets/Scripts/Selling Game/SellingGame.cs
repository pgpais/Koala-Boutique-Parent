using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellingGame : MonoBehaviour
{
    [SerializeField] float defaultGameSpeed = 1f;
    [SerializeField] float gameSpeedModifier = 2f;

    [Header("Price Modifiers (By zone)")]

    [Tooltip("Price modifier when below cheap zone")]
    [SerializeField] float tooCheapPriceModifier = 0f;

    [Tooltip("Price modifier when below cheap zone")]
    [SerializeField] float veryCheapPriceModifier = 0.5f;

    [Tooltip("Price modifier when below cheap zone")]
    [SerializeField] float justRightPriceModifier = 1f;

    [Tooltip("Price modifier when below cheap zone")]
    [SerializeField] float veryExpensivePriceModifier = 2f;

    [Tooltip("Price modifier when below cheap zone")]
    [SerializeField] float tooExpensivePriceModifier = 0f;

    [Space]

    [SerializeField] float cheapAmountModifier = 1f;
    [SerializeField] float justRightAmountModifier = 2f;
    [SerializeField] float expensiveAmountModifier = 0.5f;

    [Header("Zones")]

    [Tooltip("Below this zone, the player sells the item at 0 value")]
    [Range(0f, 0.5f)]
    [SerializeField] float minCheapSellPercentage;

    [Tooltip("If player hits this zone, increase number of items being sold next runs")]
    [Range(0f, 1f)]
    [SerializeField] float minSuccessPercentage, maxSuccessPercentage;

    [Tooltip("Above this zone, the player doesn't sell the item and decreases amount of items sold next runs")]
    [Range(0.5f, 1f)]
    [SerializeField] float maxExpensiveSellPercentage;

    [Header("References")]
    [SerializeField] Slider slider;
    [SerializeField] Image cheapZoneImage;
    [SerializeField] Image centerImage;
    [SerializeField] Image expensiveZoneImage;
    [SerializeField] Button sellButton;
    [SerializeField] Button backButton;

    [Header("UI Stuff")]
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text tooCheapPriceText;
    [SerializeField] TMP_Text veryCheapPriceText;
    [SerializeField] TMP_Text justRightPriceText;
    [SerializeField] TMP_Text veryExpensivePriceText;
    [SerializeField] TMP_Text tooExpensivePriceText;
    [SerializeField] TMP_Text amountSoldText;
    [SerializeField] TMP_Text profitText;
    [SerializeField] TMP_Text valueText;

    [Header("DEBUG")]
    private float gameSpeed;
    [SerializeField] private Item item;
    [SerializeField] private int maxAmount;
    private int currentAmount = 1;
    private int amountSold = 0;
    private int incomeSoFar = 0;

    private RectTransform cheapRect;
    private RectTransform centerRect;
    private RectTransform expensiveRect;
    private float sliderValue;
    private bool increasing;

    private Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        ChangeCheapSize();
        ChangeCenterSize();
        ChangeExpensiveSize();

        backButton.onClick.AddListener(CloseMenu);
    }

    public void Initialize(Item item, int maxAmount)
    {
        ShowMenu();

        this.item = item;
        this.maxAmount = maxAmount;
        currentAmount = 1;
        amountSold = 0;
        incomeSoFar = 0;
        gameSpeed = defaultGameSpeed;

        UpdateUI();

        coroutine = StartCoroutine(GameLoop());
    }

    // Update is called once per frame
    IEnumerator GameLoop()
    {
        sliderValue = 0.5f;
        slider.value = sliderValue;
        yield return null;

        // TODO: do countdown timer or start button;

        yield return null;
        while (true)
        {
            if (increasing)
            {
                sliderValue += gameSpeed * Time.deltaTime;

                if (sliderValue > 1)
                {
                    increasing = false;
                    sliderValue = 2 - slider.value;
                }
            }
            else
            {
                sliderValue -= gameSpeed * Time.deltaTime;

                if (sliderValue < 0)
                {
                    increasing = true;
                    sliderValue *= -1;
                }
            }

            slider.value = sliderValue;
            yield return null;
        }
    }

    private void OnEnable()
    {
        sellButton.onClick.AddListener(OnSellButton);
        if (item != null)
        {
            Initialize(item, maxAmount);
        }
    }

    private void OnDisable()
    {
        StopCoroutine(coroutine);
        sellButton.onClick.RemoveListener(OnSellButton);
    }

    void OnSellButton()
    {
        if (sliderValue < minCheapSellPercentage)
        {
            // TODO: way too cheap
            incomeSoFar += ItemManager.instance.SellItem(item.ItemName, currentAmount, tooCheapPriceModifier);
            amountSold += currentAmount;
            currentAmount = (int)(currentAmount * cheapAmountModifier);
        }
        else if (sliderValue < minSuccessPercentage)
        {
            // TODO: too cheap
            gameSpeed /= gameSpeedModifier;
            incomeSoFar += ItemManager.instance.SellItem(item.ItemName, currentAmount, veryCheapPriceModifier);
            amountSold += currentAmount;
            currentAmount = (int)(currentAmount * cheapAmountModifier);
        }
        else if (sliderValue <= maxSuccessPercentage)
        {
            // TODO: just right
            Debug.Log("Just right!");
            incomeSoFar += ItemManager.instance.SellItem(item.ItemName, currentAmount, justRightPriceModifier);
            amountSold += currentAmount;
            currentAmount = (int)(currentAmount * justRightAmountModifier);
        }
        else if (sliderValue <= maxExpensiveSellPercentage)
        {
            // TODO: too expensive
            incomeSoFar += ItemManager.instance.SellItem(item.ItemName, currentAmount, veryExpensivePriceModifier);
            gameSpeed *= gameSpeedModifier;
            amountSold += currentAmount;
            currentAmount = (int)(currentAmount * expensiveAmountModifier);
        }
        else
        {
            // TODO: don't sell, too expensive
            incomeSoFar += ItemManager.instance.SellItem(item.ItemName, 0, tooExpensivePriceModifier);
            gameSpeed *= gameSpeedModifier;
            currentAmount = (int)(currentAmount * expensiveAmountModifier);
        }


        currentAmount = Mathf.Clamp(currentAmount, 1, maxAmount - amountSold);



        UpdateUI();

        if (amountSold == maxAmount)
        {
            StopGame();
        }
    }

    void StopGame()
    {
        StopCoroutine(coroutine);
        titleText.text = $"You don't have any {item.ItemName} left to sell!";
        sellButton.onClick.RemoveListener(OnSellButton);
    }

    void ShowMenu()
    {
        gameObject.SetActive(true);
        transform.parent.gameObject.SetActive(true);
    }

    void CloseMenu()
    {
        gameObject.SetActive(false);
        transform.parent.gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        ChangeCheapSize();
        ChangeCenterSize();
        ChangeExpensiveSize();
    }

    void UpdateUI()
    {
        titleText.text = $"Selling {item.ItemName}";
        resultText.text = $"You've sold {amountSold} of {item.ItemName} for {incomeSoFar}, so far.";

        int itemPrice = item.GoldValue + MarketPrices.instance.GetCostModifierForItem(item.ItemName);

        tooCheapPriceText.text = ((int)(itemPrice * tooCheapPriceModifier)).ToString();
        veryCheapPriceText.text = ((int)(itemPrice * veryCheapPriceModifier)).ToString();
        justRightPriceText.text = ((int)(itemPrice * justRightPriceModifier)).ToString();
        veryExpensivePriceText.text = ((int)(itemPrice * veryExpensivePriceModifier)).ToString();
        tooExpensivePriceText.text = ((int)(itemPrice * tooExpensivePriceModifier)).ToString();
        valueText.text = item.GoldValue.ToString();
        amountSoldText.text = amountSold.ToString();
        profitText.text = incomeSoFar.ToString();
    }

    void ChangeCenterSize()
    {
        if (centerRect == null)
        {
            centerRect = centerImage.GetComponent<RectTransform>();
        }

        var maxAnchor = centerRect.anchorMax;
        maxAnchor.x = maxSuccessPercentage;
        var minAnchor = centerRect.anchorMin;
        minAnchor.x = minSuccessPercentage;
        centerRect.anchorMax = maxAnchor;
        centerRect.anchorMin = minAnchor;

        centerRect.offsetMax = new Vector2(0, centerRect.offsetMax.y);
        centerRect.offsetMin = new Vector2(0, centerRect.offsetMin.y);
    }
    void ChangeCheapSize()
    {
        if (cheapRect == null)
        {
            cheapRect = cheapZoneImage.GetComponent<RectTransform>();
        }

        var maxAnchor = cheapRect.anchorMax;
        maxAnchor.x = minSuccessPercentage;
        var minAnchor = cheapRect.anchorMin;
        minAnchor.x = minCheapSellPercentage;
        cheapRect.anchorMax = maxAnchor;
        cheapRect.anchorMin = minAnchor;

        cheapRect.offsetMax = new Vector2(0, cheapRect.offsetMax.y);
        cheapRect.offsetMin = new Vector2(0, cheapRect.offsetMin.y);
    }
    void ChangeExpensiveSize()
    {
        if (expensiveRect == null)
        {
            expensiveRect = expensiveZoneImage.GetComponent<RectTransform>();
        }

        var maxAnchor = expensiveRect.anchorMax;
        maxAnchor.x = maxExpensiveSellPercentage;
        var minAnchor = expensiveRect.anchorMin;
        minAnchor.x = maxSuccessPercentage;
        expensiveRect.anchorMax = maxAnchor;
        expensiveRect.anchorMin = minAnchor;

        expensiveRect.offsetMax = new Vector2(0, expensiveRect.offsetMax.y);
        expensiveRect.offsetMin = new Vector2(0, expensiveRect.offsetMin.y);
    }
}

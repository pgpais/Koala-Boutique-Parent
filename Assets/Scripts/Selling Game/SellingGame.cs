using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellingGame : MonoBehaviour
{
    [SerializeField] float defaultGameSpeed = 1f;
    [SerializeField] float gameSpeedModifier = 0.2f;
    [SerializeField] float sellValueModifier = 3f;

    [Range(0.1f, 0.5f)]
    [SerializeField] float successRange = 0.1f;

    [Header("UI References")]
    [SerializeField] Slider slider;
    [SerializeField] Button sellButton;
    [SerializeField] TMP_Text itemQuantityText;
    [SerializeField] TMP_Text goldObtainedText;
    [SerializeField] TMP_Text itemNameText;
    [SerializeField] Image itemImage;

    private Item item;
    private int quantity;
    private int goldObtained;

    private float gameSpeed;
    private float sliderValue;
    private bool increasing;

    private void Awake()
    {
        sellButton.onClick.AddListener(OnSellButtonClicked);
    }

    public void Init(Item item, int quantity)
    {
        ShowScreen(true);

        this.item = item;
        this.quantity = quantity;
        this.goldObtained = 0;
        this.gameSpeed = defaultGameSpeed;

        itemNameText.text = item.ItemName;
        itemImage.sprite = item.ItemSprite;

        UpdateUI();

        sellButton.interactable = true;

        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        sliderValue = 0.5f;
        slider.value = sliderValue;
        sellButton.interactable = true;
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

    public void OnSellButtonClicked()
    {
        float sellingModifier;

        bool inRange = sliderValue <= 0.5f + successRange && sliderValue >= 0.5f - successRange;

        if (inRange)
        {
            sellingModifier = sellValueModifier;
            gameSpeed += gameSpeedModifier;
        }
        else
        {
            // Apply partial price modifier based on distance from center
            float distFromCenterNormalized = Mathf.Abs(sliderValue - 0.5f) / 0.5f;
            sellingModifier = sellValueModifier * (1 - distFromCenterNormalized);
            gameSpeed -= gameSpeedModifier;
        }


        int sellValue = ItemManager.instance.SellItem(item, 1, sellingModifier);

        Debug.Log($"Sold {item.ItemName} at {sellValue}");
        goldObtained += sellValue;

        quantity--;
        if (quantity == 0)
        {
            sellButton.interactable = false;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        itemQuantityText.text = quantity.ToString();
        goldObtainedText.text = goldObtained.ToString();
    }

    void ShowScreen(bool show)
    {
        gameObject.SetActive(show);
        transform.parent.gameObject.SetActive(show);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

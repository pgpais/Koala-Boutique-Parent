using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockableItemCost : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemText;
    [SerializeField] Color enoughItemColor;
    [SerializeField] Slider slider;

    private Item item;
    private int targetAmount;

    internal void InitUI(Transform parent, Sprite sprite, string name, int amount)
    {
        this.item = null;
        this.targetAmount = amount;

        transform.SetParent(parent, false);
        itemImage.sprite = sprite;
        itemText.text = $"{amount}× {name}";

        if (name == "Coins")
        {
            if (GoldManager.instance.CurrentGold >= targetAmount)
            {
                itemText.color = enoughItemColor;
            }
            HandleSliderForGold();
            GoldManager.GoldChanged.AddListener((amount) => HandleSliderForGold());
        }
        else if (name == "Gems")
        {
            if (GoldManager.instance.CurrentGems >= targetAmount)
            {
                itemText.color = enoughItemColor;
            }
            HandleSliderForGems();
            GoldManager.GemChanged.AddListener((amount) => HandleSliderForGems());
        }
    }

    internal void InitUI(Transform parent, Item item, int amount)
    {
        this.item = item;
        this.targetAmount = amount;

        transform.SetParent(parent, false);
        itemImage.sprite = item.ItemSprite;
        itemText.text = $"{amount}× {item.ItemName}";

        if (ItemManager.instance.HasEnoughItem(item.ItemName, amount))
        {
            itemText.color = enoughItemColor;
        }

        HandleSlider();
        ItemManager.ItemUpdated.AddListener((item, amount) =>
        {
            if (item.ItemName == this.item.ItemName) HandleSlider();
        });
    }

    void HandleSlider()
    {
        if (item == null)
            return;

        int amount = 0;
        if (ItemManager.instance.itemQuantity.ContainsKey(item.ItemName))
        {
            amount = ItemManager.instance.itemQuantity[item.ItemName];
        }

        float value = amount / targetAmount;
        value = Mathf.Clamp01(value);
        slider.value = value;
    }

    void HandleSliderForGold()
    {
        int amount = GoldManager.instance.CurrentGold;

        float value = amount / targetAmount;
        value = Mathf.Clamp01(value);
        slider.value = value;
    }

    void HandleSliderForGems()
    {
        int amount = GoldManager.instance.CurrentGems;

        float value = amount / targetAmount;
        value = Mathf.Clamp01(value);
        slider.value = value;
    }
}

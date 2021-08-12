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

        if (name == "Coins")
        {
            itemText.text = $"{amount}× {Localisation.Get(StringKey.Item_Coin_Name)}";
            if (GoldManager.instance.CurrentGold >= targetAmount)
            {
                itemText.color = enoughItemColor;
            }
            HandleSliderForGold();
            GoldManager.GoldChanged.AddListener((amount) => HandleSliderForGold());
        }
        else if (name == "Gems")
        {
            itemText.text = $"{amount}× {Localisation.Get(StringKey.Item_Gem_Name)}";
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
        itemText.text = $"{amount}× {Localisation.Get(item.ItemNameStringKey)}";

        if (ItemManager.instance.HasEnoughItem(item.ItemNameKey, amount))
        {
            itemText.color = enoughItemColor;
        }

        HandleSlider();
        ItemManager.ItemUpdated.AddListener((item, amount) =>
        {
            if (item.ItemNameKey == this.item.ItemNameKey) HandleSlider();
        });
    }

    void HandleSlider()
    {
        if (item == null)
            return;

        int amount = 0;
        if (ItemManager.instance.itemQuantity.ContainsKey(item.ItemNameKey))
        {
            amount = ItemManager.instance.itemQuantity[item.ItemNameKey];
        }

        float value = (float)amount / (float)targetAmount;
        value = Mathf.Clamp01(value);
        slider.value = value;

        if (amount == targetAmount)
        {
            itemText.color = enoughItemColor;
        }
    }

    void HandleSliderForGold()
    {
        int amount = GoldManager.instance.CurrentGold;

        float value = (float)amount / (float)targetAmount;
        value = Mathf.Clamp01(value);
        slider.value = value;

        if (value == targetAmount)
        {
            itemText.color = enoughItemColor;
        }
    }

    void HandleSliderForGems()
    {
        int amount = GoldManager.instance.CurrentGems;

        float value = (float)amount / (float)targetAmount;
        value = Mathf.Clamp01(value);
        slider.value = value;

        if (value == targetAmount)
        {
            itemText.color = enoughItemColor;
        }
    }
}

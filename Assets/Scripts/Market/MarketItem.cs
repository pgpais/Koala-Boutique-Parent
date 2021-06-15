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

    [SerializeField] Button button;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            if (!button.interactable)
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
        button.interactable = AvailableItem;
        fadeImage.enabled = !AvailableItem;
    }

    public void UpdateUI(string itemName, int itemValue, Sprite itemImage)
    {
        this.itemName.text = itemName;
        this.itemValue.text = itemValue.ToString();

        if (itemImage != null)
            this.itemImage.sprite = itemImage;

        AvailableItem = ItemManager.instance.itemQuantity.ContainsKey(itemName);
        button.interactable = AvailableItem;
        fadeImage.enabled = !AvailableItem;
    }

    public void UpdateValue(int itemValue)
    {
        this.itemValue.text = itemValue.ToString();
    }
}

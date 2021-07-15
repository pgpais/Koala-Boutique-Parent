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

    internal void InitUI(Transform parent, Sprite sprite, string name, int amount)
    {
        transform.SetParent(parent, false);
        itemImage.sprite = sprite;
        itemText.text = $"{amount}× {name}";
    }

    internal void InitUI(Transform parent, Item item, int amount)
    {
        transform.SetParent(parent, false);
        itemImage.sprite = item.ItemSprite;
        itemText.text = $"{amount}× {item.ItemName}";
    }
}

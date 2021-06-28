using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SmallItemUI : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] Image image;

    public void InitUI(Item item, int amount)
    {
        text.text = $"{amount}x {item.ItemName}";
        image.sprite = item.ItemSprite;
    }
}

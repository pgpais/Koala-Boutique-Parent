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
        if (text != null)
            text.text = $"{amount}";

        if (image != null)
            image.sprite = item.ItemSprite;
    }

    public void InitUI(Item item)
    {
        if (image != null)
        {
            image.sprite = item.ItemSprite;
        }
    }
}

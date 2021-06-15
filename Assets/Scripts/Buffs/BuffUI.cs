using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffUI : MonoBehaviour
{
    public string BuffName => buffName.text;

    [SerializeField] Image buffImage;
    [SerializeField] TMP_Text buffName;
    [SerializeField] TMP_Text buffDescription;
    [SerializeField] Toggle buyToggle;
    [SerializeField] TMP_Text buffPrice;

    public void Init(Buff buff, ToggleGroup supportBuffsGroup)
    {
        buffImage.sprite = buff.icon;
        buffName.text = buff.buffName;
        buffDescription.text = buff.description;
        buffPrice.text = buff.price.ToString();
        supportBuffsGroup.RegisterToggle(buyToggle);
    }

    private void OnEnable()
    {
        buyToggle.onValueChanged.AddListener(ToggleBuy);
    }

    private void OnDisable()
    {
        buyToggle.onValueChanged.RemoveListener(ToggleBuy);
    }

    void ToggleBuy(bool toggled)
    {
        Debug.Log($"Deciding to {(toggled ? "" : "not ")}buy {buffName.text}");
    }
}

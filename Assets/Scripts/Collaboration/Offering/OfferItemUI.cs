using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class OfferItemUI : MonoBehaviour
{
    public bool isOn => offerToggle.isOn;

    public string ItemName => item.ItemName;

    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemNameText;

    Toggle offerToggle;
    Item item;

    void Awake()
    {
        offerToggle = GetComponent<Toggle>();
    }

    public void Init(Item item)
    {
        this.item = item;
        itemImage.sprite = item.ItemSprite;
        itemNameText.text = item.ItemName;
        offerToggle.isOn = false;
    }

}
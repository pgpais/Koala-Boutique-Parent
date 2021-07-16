using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class OfferItemUI : MonoBehaviour
{
    public UnityEvent<Item> OnSelected = new UnityEvent<Item>();
    public bool isOn => offerToggle.isOn;
    public Toggle Toggle => offerToggle;

    public string ItemName => item.ItemName;

    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemNameText;

    Toggle offerToggle;
    Item item;

    void Awake()
    {
        offerToggle = GetComponent<Toggle>();
    }

    public void Init(Item item, ToggleGroup toggleGroup = null)
    {
        this.item = item;
        itemImage.sprite = item.ItemSprite;
        itemNameText.text = item.ItemName;
        offerToggle.isOn = false;
        offerToggle.group = toggleGroup;

        offerToggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn) OnSelected.Invoke(item);
        });
    }

}
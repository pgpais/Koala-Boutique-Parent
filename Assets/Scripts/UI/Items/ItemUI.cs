using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] TMP_Text itemNameText;
    [SerializeField] TMP_Text itemDescriptionText;
    [SerializeField] TMP_Text itemQuantityText;
    [SerializeField] Image itemImage;
    [SerializeField] Button startProcessingItemButton;

    public void Init(Item item, int itemQuantity)
    {
        itemNameText.text = item.ItemName;
        itemDescriptionText.text = item.Description;
        itemQuantityText.text = itemQuantity.ToString();

        // TODO: #13 Move item event listeners to parent UI script
        ItemManager.ItemUpdated.AddListener(UpdateUI);
        ItemManager.ItemRemoved.AddListener((item) =>
        {
            if (item.ItemName == itemNameText.text)
                Destroy(gameObject);
        });
        startProcessingItemButton.onClick.AddListener(ShowStartProcessScreen);

        bool canItemBeProcessed = !(item.Type == Item.ItemType.Processed || item.Type == Item.ItemType.Valuable);
        if (!canItemBeProcessed)
            startProcessingItemButton.gameObject.SetActive(false);
    }

    private void UpdateUI(Item item, int quantity)
    {
        if (item.ItemName == itemNameText.text)
            itemQuantityText.text = quantity.ToString();
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

    private void ShowStartProcessScreen()
    {

        ProcessingManager.instance.ShowNewProcessMenu(itemNameText.text, 0, ItemManager.instance.itemQuantity[itemNameText.text]);
    }
}

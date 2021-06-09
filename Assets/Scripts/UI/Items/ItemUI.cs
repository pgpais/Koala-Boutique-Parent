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
    [SerializeField] Button sellItemButton;

    private Item item;

    public void Init(Item item, int itemQuantity)
    {
        this.item = item;

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
        // TODO: #25 Make amount selection screen (copy from processScreen)
        sellItemButton.onClick.AddListener(ShowSellScreen);

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

    private void ShowSellScreen()
    {
        ItemManager.instance.ShowSellMenu(item.ItemName, 0, ItemManager.instance.itemQuantity[item.ItemName]);
    }
}

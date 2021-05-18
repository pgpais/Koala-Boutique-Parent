using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] TMP_Text itemNameText;
    [SerializeField] TMP_Text itemDescriptionText;
    [SerializeField] TMP_Text itemQuantityText;
    [SerializeField] Image itemImage;

    public void Init(Item item, int itemQuantity)
    {
        itemNameText.text = item.ItemName;
        itemDescriptionText.text = item.Description;
        itemQuantityText.text = itemQuantity.ToString();

        // TODO: #13 Move item event listeners to parent UI script
        item.ItemUpdated.AddListener(UpdateUI);
        item.ItemRemoved.AddListener(() => Destroy(gameObject));
    }

    private void UpdateUI(int quantity)
    {
        itemQuantityText.text = quantity.ToString();
    }
}

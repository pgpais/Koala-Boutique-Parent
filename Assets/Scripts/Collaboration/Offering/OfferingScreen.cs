using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfferingScreen : MonoBehaviour
{
    [SerializeField] Transform itemList;
    [SerializeField] Button offeringButton;
    [SerializeField] Button cancelButton;
    [SerializeField] OfferItemUI offeringItemPrefab;

    List<OfferItemUI> offerItemList = new List<OfferItemUI>();

    private void Awake()
    {
        offeringButton.onClick.AddListener(MakeOffer);
        cancelButton.onClick.AddListener(CloseScreen);
    }

    private void OnEnable()
    {
        var items = ItemManager.instance.itemsData.GetUnlockedItems();
        foreach (var item in items)
        {
            if (ItemManager.instance.HasEnoughItem(item.ItemName, 1))
            {
                var offerItem = Instantiate(offeringItemPrefab, itemList);
                offerItem.Init(item);
                offerItem.gameObject.SetActive(true);
                offerItemList.Add(offerItem);
            }
        }
    }

    private void OnDisable()
    {
        // Destroy every child object of itemList
        foreach (Transform child in itemList)
        {
            Destroy(child.gameObject);
        }
    }

    void MakeOffer()
    {
        List<string> itemsToOffer = new List<string>();

        foreach (var item in offerItemList)
        {
            if (item.isOn)
            {
                itemsToOffer.Add(item.ItemName);
            }
        }

        // TODO: Make offer
        if (OfferingManager.Instance.MakeOffer(itemsToOffer))
        {
            // TODO: Good offer
            Debug.Log("Offering made");
        }
        else
        {
            // TODO: Bad offer
            Debug.Log("Offering failed");
        }

        CloseScreen();
    }

    void CloseScreen()
    {
        transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}

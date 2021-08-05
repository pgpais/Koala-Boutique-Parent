using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OfferingScreen : MonoBehaviour
{
    [SerializeField] Transform itemList;
    [SerializeField] Button offeringButton;
    [SerializeField] Button cancelButton;
    [SerializeField] OfferItemUI offeringItemPrefab;
    [SerializeField] OfferingResult offeringResultScreen;

    List<OfferItemUI> offerItemList = new List<OfferItemUI>();

    private void Awake()
    {
        offeringButton.onClick.AddListener(MakeOffer);
        cancelButton.onClick.AddListener(CloseScreen);
    }

    private void OnEnable()
    {
        MenuSwitcher.instance.ShowFadePanel();
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

        bool isSuccess = OfferingManager.Instance.MakeOffer(itemsToOffer);

        if (isSuccess)
        {
            // TODO: #66 Good offer
            Debug.Log("Offering made");
        }
        else
        {
            // TODO: #67 Bad offer
            Debug.Log("Offering failed");
        }

        CloseScreen();
        ShowResultScreen(itemsToOffer.Select(itemName => ItemManager.instance.itemsData.GetItemByName(itemName)).ToList(), isSuccess);
    }

    void CloseScreen()
    {
        transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    void ShowResultScreen(List<Item> itemsToOffer, bool isSuccess)
    {
        if (isSuccess)
        {
            offeringResultScreen.Init(itemsToOffer, isSuccess);
        }
        else
        {
            offeringResultScreen.Init(itemsToOffer, isSuccess, OfferingManager.failedOfferingPenalty);
        }
    }
}

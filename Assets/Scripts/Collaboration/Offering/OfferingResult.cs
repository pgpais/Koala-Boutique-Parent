using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfferingResult : MonoBehaviour
{
    [SerializeField] string successMessage;
    [SerializeField] string failureMessage;
    [Space]
    [SerializeField] SmallItemUI successItem;
    [SerializeField] SmallItemUI failureItem;
    [SerializeField] LayoutGroup itemsLayoutGroup;
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text coinLossText;

    public void Init(List<Item> offeringItems, bool isSuccess, int coinLoss = 0)
    {
        transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);

        resultText.text = isSuccess ? successMessage : failureMessage;

        coinLossText.transform.parent.gameObject.SetActive(!isSuccess);
        coinLossText.text = coinLoss.ToString();

        //Remove childs of itemsLayoutGroup
        foreach (Transform child in itemsLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in offeringItems)
        {
            var itemUI = Instantiate(isSuccess ? successItem : failureItem);
            itemUI.transform.SetParent(itemsLayoutGroup.transform, false);
            itemUI.InitUI(item);
        }
    }

    private void OnEnable()
    {
        MenuSwitcher.instance.ShowFadePanel();
    }
}

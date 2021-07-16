using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AbundantGatherableScreen : MonoBehaviour
{
    public UnityEvent<Item> OnAbundantGatherableSelected = new UnityEvent<Item>();
    [SerializeField] OfferItemUI itemPrefab;
    [Space]
    [SerializeField] Transform layoutGroup;
    [SerializeField] ToggleGroup toggleGroup;

    public void Show()
    {
        transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);
        PopulateList();
    }

    public void Hide()
    {
        StartCoroutine(ClearList());
        transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    void PopulateList()
    {
        List<Item> unlockedGatherables = ItemManager.instance.itemsData.GetUnlockedItems().FindAll(item => item.Type == Item.ItemType.Gatherable);

        foreach (var item in unlockedGatherables)
        {
            OfferItemUI itemUI = Instantiate(itemPrefab);
            itemUI.transform.SetParent(layoutGroup, false);
            itemUI.Init(item, toggleGroup);
            itemUI.OnSelected.AddListener(OnItemSelected);
        }
    }

    private IEnumerator ClearList()
    {
        foreach (Transform child in layoutGroup)
        {
            child.GetComponent<OfferItemUI>().OnSelected.RemoveListener(OnItemSelected);
            Destroy(child.gameObject);
            yield return null;
        }
    }

    public void OnItemSelected(Item item)
    {
        OnAbundantGatherableSelected.Invoke(item);
    }
}

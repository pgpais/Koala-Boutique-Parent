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

    private List<OfferItemUI> items = new List<OfferItemUI>();

    private void Awake()
    {
        PopulateList();
    }

    public void Show()
    {
        transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);

        foreach (var item in items)
        {
            item.Show();
        }
    }

    public void Hide()
    {
        transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void PopulateList()
    {
        List<Item> gatherables = ItemManager.instance.itemsData.Items.FindAll(item => item.Type == Item.ItemType.Gatherable);
        items = new List<OfferItemUI>();


        foreach (Transform child in layoutGroup)
        {
            items.Add(child.GetComponent<OfferItemUI>());
        }

        if (items.Count != gatherables.Count)
        {
            Debug.LogError("Wrong child count!");
        }

        for (int i = 0; i < gatherables.Count; i++)
        {
            Item gatherable = gatherables[i];

            items[i].Init(gatherable);
            items[i].OnSelected.AddListener(OnItemSelected);
        }
    }

    public IEnumerator ClearList()
    {
        foreach (Transform child in layoutGroup)
        {
            OfferItemUI itemUI = child.GetComponent<OfferItemUI>();
            itemUI.OnSelected.RemoveListener(OnItemSelected);
            itemUI.Remove();

            yield return null;
        }

        toggleGroup.enabled = false;
    }

    public void OnItemSelected(Item item)
    {
        OnAbundantGatherableSelected.Invoke(item);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DanielLochner.Assets.SimpleScrollSnap;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HomeScreen : SerializedMonoBehaviour
{
    public static UnityEvent<Item, int> OnDiseasedItemReceived = new UnityEvent<Item, int>();

    public static string newItemsReferenceName = "newItems";

    [SerializeField] Image newItemsTitleImage;
    [SerializeField] Dictionary<Language, Sprite> newItemsTitleSprites = new Dictionary<Language, Sprite>();
    [SerializeField] NewItemsPanel diseasedNewItemsPanel;
    [SerializeField] NewItemsPanel noDiseasedNewItemsPanel;
    [SerializeField] Transform contentParent;
    [SerializeField] Transform paginationParent;
    [SerializeField] GameObject pagination;
    [SerializeField] SimpleScrollSnap scrollSnap;


    NewItemsList newItemsList;

    void Awake()
    {
        GetNewItemsList();
    }

    private void GetNewItemsList()
    {
        FirebaseCommunicator.instance.GetObject(newItemsReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error: " + task.Exception);
            }
            else
            {
                string json = task.Result.GetRawJsonValue();
                if (string.IsNullOrEmpty(json))
                {
                    Debug.Log("No new items");

                }
                else
                {
                    newItemsList = JsonConvert.DeserializeObject<NewItemsList>(json);

                    FirebaseCommunicator.instance.RemoveObject(newItemsReferenceName, (task, obj) =>
                    {
                        Debug.Log("Removed new items");
                    });
                }

                UpdateUI();
            }
        });
    }

    private void UpdateUI()
    {
        newItemsTitleImage.sprite = newItemsTitleSprites[Localisation.currentLanguage];

        int itemsLeft = newItemsList.lootedItems.Count;

        Dictionary<Item, int> itemCounts = new Dictionary<Item, int>();
        foreach (var itemQuantity in newItemsList.lootedItems)
        {
            var item = ItemManager.instance.itemsData.GetItemByName(itemQuantity.Key);
            itemCounts.Add(item, itemQuantity.Value);
        }

        if (scrollSnap.Panels != null)
        {
            for (int i = 0; i < scrollSnap.Panels.Length; i++)
            {
                scrollSnap.Remove(i);
            }
        }

        bool spawnedDiseased = false;
        while (itemCounts.Keys.Count != 0)
        {

            if (newItemsList.diseasedGoldLoss < 0 && !spawnedDiseased)
            {
                NewItemsPanel panel = Instantiate(diseasedNewItemsPanel);

                Item diseasedItem = ItemManager.instance.itemsData.GetItemByName(newItemsList.diseasedItemName);

                GameObject pag = Instantiate(pagination);
                pag.transform.SetParent(paginationParent, false);

                panel.transform.SetParent(contentParent, false);
                scrollSnap.AddToBack(panel.gameObject);
                panel.Init(newItemsList.diseasedGoldLoss, diseasedItem, itemCounts);

                OnDiseasedItemReceived.Invoke(diseasedItem, newItemsList.diseasedGoldLoss);

                spawnedDiseased = true;
            }
            else
            {
                NewItemsPanel panel = Instantiate(noDiseasedNewItemsPanel);

                GameObject pag = Instantiate(pagination);
                pag.transform.SetParent(paginationParent, false);

                panel.transform.SetParent(contentParent, false);
                panel.Init(newItemsList.diseasedGoldLoss, null, itemCounts);

                scrollSnap.AddToBack(panel.gameObject);
            }
        }


    }

    class NewItemsList
    {
        public int diseasedGoldLoss;
        public string diseasedItemName;
        public Dictionary<string, int> lootedItems;

        public NewItemsList(int diseasedGoldLoss, Dictionary<string, int> lootedItems, string diseasedItemName)
        {
            this.diseasedGoldLoss = diseasedGoldLoss;
            this.lootedItems = lootedItems;
            this.diseasedItemName = diseasedItemName;
        }

        internal NewItemsList Merge(NewItemsList oldNewItemsList)
        {
            diseasedGoldLoss += oldNewItemsList.diseasedGoldLoss;
            foreach (var lootedItem in oldNewItemsList.lootedItems)
            {
                if (lootedItems.ContainsKey(lootedItem.Key))
                {
                    lootedItems[lootedItem.Key] += lootedItem.Value;
                }
                else
                {
                    lootedItems.Add(lootedItem.Key, lootedItem.Value);
                }
            }
            return this;
        }
    }
}

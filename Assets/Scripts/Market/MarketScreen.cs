using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MarketScreen : MonoBehaviour
{
    [SerializeField] MarketItem marketItemPrefab;
    [SerializeField] Transform marketItemGroup;
    [SerializeField] TMP_Text menuTitle;
    [SerializeField] StringKey marketTitleStringKey;

    Dictionary<string, MarketItem> marketItems;

    void Awake()
    {
        marketItems = new Dictionary<string, MarketItem>();

        // FirebaseCommunicator.LoggedIn.AddListener(Init);
        FirebaseCommunicator.LoggedOut.AddListener(OnLogout);

        // MarketPrices.GotMarketPrices.AddListener(() =>
        // {
        //     Init();

        //     ItemsList items = ItemManager.instance.itemsData;

        //     foreach (var marketItem in marketItems.Values)
        //     {
        //         Item item = items.GetItemByName(marketItem.ItemName);
        //         int valueModifier = MarketPrices.instance.GetCostModifierForItem(item.name);

        //         marketItem.UpdateValue(item.GoldValue + valueModifier);
        //     }
        // });
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();

        ItemsList items = ItemManager.instance.itemsData;

        foreach (var marketItem in marketItems.Values)
        {
            Item item = items.GetItemByName(marketItem.ItemNameKey);
            int valueModifier = MarketPrices.instance.GetCostModifierForItem(item.name);

            marketItem.UpdateValue(item.GoldValue + valueModifier);
        }
    }

    void Init()
    {
        List<Item> sellableItems = ItemManager.instance.itemsData.Items.Where(item => item.Type == Item.ItemType.Processed || item.Type == Item.ItemType.Gatherable || item.ProcessResult == null).ToList();

        foreach (var item in sellableItems)
        {
            MarketItem marketItem = Instantiate(marketItemPrefab, marketItemGroup);
            marketItem.Init(item.ItemNameKey, item.GoldValue, item.ItemSprite);

            if (MarketPrices.instance.hasPrices)
            {
                int costModifier = MarketPrices.instance.GetCostModifierForItem(item.ItemNameKey);
                marketItem.UpdateValue(item.GoldValue + costModifier);
            }

            marketItems.Add(item.ItemNameKey, marketItem);
            if (!item.Unlocked)
            {
                marketItem.gameObject.SetActive(false);
                Item.ItemUnlocked.AddListener((unlockedItem) =>
                {
                    if (item == unlockedItem)
                        marketItem.gameObject.SetActive(true);
                });
            }
            //if name is encrypted key or decrypted key don't show
            if (item.ItemNameKey.Contains("Key"))
            {
                marketItem.gameObject.SetActive(false);
            }
            SortMarketItems();
        }

        ItemManager.NewItemAdded.AddListener((item, quantity) =>
       {
           if (!marketItems.ContainsKey(item.ItemNameKey))
           {
               return;
           }

           int valueModifier = 0;
           if (MarketPrices.instance.hasPrices)
               valueModifier = MarketPrices.instance.GetCostModifierForItem(item.name);

           marketItems[item.ItemNameKey].UpdateUI(item.ItemName, item.GoldValue + valueModifier, item.ItemSprite);
           SortMarketItems();
       });

        ItemManager.ItemRemoved.AddListener((item) =>
        {
            if (!marketItems.ContainsKey(item.ItemNameKey))
            {
                return;
            }

            int valueModifier = 0;
            if (MarketPrices.instance.hasPrices)
                valueModifier = MarketPrices.instance.GetCostModifierForItem(item.name);

            marketItems[item.ItemNameKey].UpdateUI(item.ItemName, item.GoldValue + valueModifier, item.ItemSprite);
            SortMarketItems();
        });

        ItemManager.ItemUpdated.AddListener(UpdateUI);
    }

    private void OnEnable()
    {
        menuTitle.text = Localisation.Get(marketTitleStringKey);

        SortMarketItems();
    }

    void UpdateUI(Item item, int amount)
    {
        if (marketItems.ContainsKey(item.ItemNameKey))
        {
            marketItems[item.ItemNameKey].UpdateAmount(amount);
        }
    }

    void SortMarketItems()
    {
        List<MarketItem> marketItems = this.marketItems.Values.ToList();
        marketItems.Sort((a, b) => a.CompareTo(b));

        foreach (var item in marketItems)
        {
            item.transform.SetSiblingIndex(marketItems.IndexOf(item));
        }
    }

    void OnLogout()
    {
        foreach (var marketItem in marketItems)
        {
            Destroy(marketItem.Value.gameObject);
        }
        marketItems = new Dictionary<string, MarketItem>();
    }
}

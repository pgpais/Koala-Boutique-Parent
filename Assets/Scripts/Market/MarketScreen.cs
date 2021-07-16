using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketScreen : MonoBehaviour
{
    [SerializeField] MarketItem marketItemPrefab;
    [SerializeField] Transform marketItemGroup;

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
            Item item = items.GetItemByName(marketItem.ItemName);
            int valueModifier = MarketPrices.instance.GetCostModifierForItem(item.name);

            marketItem.UpdateValue(item.GoldValue + valueModifier);
        }
    }

    void Init()
    {
        foreach (var item in ItemManager.instance.itemsData.Items)
        {
            MarketItem marketItem = Instantiate(marketItemPrefab, marketItemGroup);
            marketItem.Init(item.ItemName, item.GoldValue, item.ItemSprite);

            if (MarketPrices.instance.hasPrices)
            {
                int costModifier = MarketPrices.instance.GetCostModifierForItem(item.ItemName);
                marketItem.UpdateValue(item.GoldValue + costModifier);
            }

            marketItems.Add(item.ItemName, marketItem);
        }

        ItemManager.NewItemAdded.AddListener((item, quantity) =>
       {
           int valueModifier = 0;
           if (MarketPrices.instance.hasPrices)
               valueModifier = MarketPrices.instance.GetCostModifierForItem(item.name);

           marketItems[item.ItemName].UpdateUI(item.ItemName, item.GoldValue + valueModifier, item.ItemSprite);
       });

        ItemManager.ItemRemoved.AddListener((item) =>
        {
            int valueModifier = 0;
            if (MarketPrices.instance.hasPrices)
                valueModifier = MarketPrices.instance.GetCostModifierForItem(item.name);

            marketItems[item.ItemName].UpdateUI(item.ItemName, item.GoldValue + valueModifier, item.ItemSprite);
        });
    }

    private void OnEnable()
    {
        //todo: #65 sort children by available then by name
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SortMarketItems()
    {

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

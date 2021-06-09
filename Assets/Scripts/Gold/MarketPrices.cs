using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
public class MarketPrices : SerializedMonoBehaviour
{
    public static string referenceName = "marketPrices";
    // TODO: hardcoded market prices by day (for a month? repeat?)

    /*
    marketCosts: {
        "Monday":{
            "0": {
                "item1": 123,
                "item2": 321
            },
            "1": {
                "item1": 143,
                "item2": 327
            }
        },
        "Tuesday":{
            "item1": 121,
            "item2": 220
        }
    }
    */
    [SerializeField] List<Dictionary<string, int>> costModifierToday;

    [SerializeField] bool testUpload;

    private int indexOfActiveCosts = 0;
    private DateTime curDay;

    private void Awake()
    {
        FirebaseCommunicator.LoggedIn.AddListener(GetPricesForToday);
    }

    private void Start()
    {
        Debug.Log(DateTime.Now.ToString("yyyyMMdd"));
        Debug.Log(DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd", null));
    }

    private void Update()
    {
        var today = DateTime.Now;

        if (testUpload)
        {
            testUpload = false;
            CreateMarketPricesForDay(today);
        }

        indexOfActiveCosts = today.Hour / 3;

        if (today.Day > curDay.Day)
        {
            GetPricesForToday();
        }
    }

    public void GetPricesForToday()
    {
        FirebaseCommunicator.instance.GetObject(new string[] { referenceName, DateTime.Now.ToString("yyyyMMdd") }, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
                return;
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey got marketPrices");
                string json = task.Result.GetRawJsonValue();
                costModifierToday = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(json);
                curDay = DateTime.ParseExact(task.Result.Key, "yyyyMMdd", null);
            }
        });
    }

    public static List<Dictionary<string, int>> GenerateMarketPrices()
    {
        List<Dictionary<string, int>> newDayPrices = new List<Dictionary<string, int>>();

        System.Random ran = new System.Random();

        for (var i = 0; i < 8; i++)
        {
            Dictionary<string, int> itemPrices = new Dictionary<string, int>();

            foreach (var item in ItemManager.instance.itemsData.Items)
            {
                itemPrices.Add(item.ItemName, ran.Next(item.MinModifier, item.MaxModifier));
            }
            newDayPrices.Add(itemPrices);
        }

        return newDayPrices;
    }

    public static void CreateMarketPricesForDay(DateTime date)
    {
        var marketPrices = GenerateMarketPrices();
        string dateString = date.ToString("yyyyMMdd");
        string json = JsonConvert.SerializeObject(marketPrices);
        Debug.Log(dateString + ":" + json);
        FirebaseCommunicator.instance.SendObject(json, new string[] { referenceName, dateString }, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong at reference." + task.Exception.ToString());
                return;
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey updated marketPrices");
            }
        });
    }


}

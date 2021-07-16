using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public class DiseasedItemsManager : MonoBehaviour
{
    public UnityEvent<Item> OnGotDiseasedItem = new UnityEvent<Item>();
    public static DiseasedItemsManager instance;
    public static string referenceName = "diseasedItems";

    public bool GotDiseasedItem { get; private set; } = false;

    [SerializeField] bool testUpload;
    [SerializeField] int howManyDays = 1;

    private Item diseasedItem;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        FirebaseCommunicator.LoggedIn.AddListener(GetDiseasedItem);
    }

    void GetDiseasedItem()
    {
        FirebaseCommunicator.instance.GetObject(new string[] { referenceName, FirebaseCommunicator.instance.FamilyId.ToString(), DateTime.Now.ToString("yyyyMMdd") }, (task) =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("smth went wrong. " + task.Exception.ToString());
                    return;
                }

                if (task.IsCompleted)
                {
                    Debug.Log("yey got diseasedItems");
                    string json = task.Result.GetRawJsonValue();
                    if (string.IsNullOrEmpty(json))
                    {
                        Debug.Log("json is empty");
                        PickDiseasedItemForDay(DateTime.Today);
                    }
                    else
                    {
                        Debug.Log("Diseased item: " + diseasedItem.ItemName);
                        diseasedItem = JsonConvert.DeserializeObject<Item>(json);
                    }
                }
            });
    }

    // Update is called once per frame
    void Update()
    {
        var today = DateTime.Now;

        if (testUpload)
        {
            testUpload = false;
            for (int i = 0; i < howManyDays; i++)
            {
                var daySpan = new TimeSpan(i, 0, 0, 0, 0);
                PickDiseasedItemForDay(today + daySpan);
            }
        }
    }

    void PickDiseasedItemForDay(DateTime date)
    {
        Random rand = new Random(date.DayOfYear);
        var items = ItemManager.instance.itemsData.Items;
        string diseasedItemName = items[rand.Next(items.Count)].ItemName;

        UploadNewDiseasedItem(diseasedItemName, date);
    }

    void UploadNewDiseasedItem(string diseasedItemName, DateTime date)
    {
        string dateString = date.ToString("yyyyMMdd");
        string json = JsonConvert.SerializeObject(diseasedItemName);
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
                   Debug.Log("yey updated diseasedItem");
               }
           });
    }
}

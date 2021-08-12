using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class DiseasedManager : MonoBehaviour
{
    public static UnityEvent OnGotDiseased = new UnityEvent();

    public static string dateFormat = "yyyyMMdd HH";
    public static string diseasedReferenceName = "diseasedItems";
    public static int diseasedHourFrequency = 8;
    public static DiseasedManager instance;

    public bool GotDiseased { get; private set; } = false;
    public Item DiseasedItem => diseasedItem;

    Item diseasedItem;
    DateTime diseasedDate;

    private void Awake()
    {
        // FirebaseCommunicator.LoggedIn.AddListener(OnLoggedIn);
        // ItemManager.OnGotItems.AddListener(OnGotItems);
        UnlockablesManager.OnGotUnlockables.AddListener(OnGotUnlocks);

        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }


    }

    private void OnGotUnlocks()
    {
        GetDiseasedItem();
    }

    private void GetDiseasedItem()
    {
        GetCurrentDate();

        FirebaseCommunicator.instance.GetObject(new string[] { diseasedReferenceName, FirebaseCommunicator.instance.FamilyId.ToString(), diseasedDate.ToString(dateFormat) }, (task) =>
           {
               if (task.IsFaulted)
               {
                   Debug.LogError(task.Exception.InnerException.Message);
               }
               else
               {
                   string json = task.Result.GetRawJsonValue();
                   if (string.IsNullOrEmpty(json))
                   {
                       Debug.Log("No diseased item found");
                       if (ShouldCreateDiseasedItem())
                       {
                           CreateDiseasedItem();
                           SendDiseasedItem();
                       }
                   }
                   else
                   {
                       string diseasedItemName = JsonConvert.DeserializeObject<string>(json);

                       diseasedItem = ItemManager.instance.itemsData.GetItemByName(diseasedItemName);
                   }
                   GotDiseased = true;
                   OnGotDiseased.Invoke();
               }
           });
    }

    private bool ShouldCreateDiseasedItem()
    {
        List<Item> unlockedGatherables = ItemManager.instance.itemsData.Items.Where(item => item.Type == Item.ItemType.Gatherable && item.Unlocked).ToList();

        if (unlockedGatherables.Count < 2)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void CreateDiseasedItem()
    {
        List<Item> gatherables = ItemManager.instance.itemsData.Items.FindAll((item) => item.Type == Item.ItemType.Gatherable && item.Unlocked);
        diseasedItem = gatherables[UnityEngine.Random.Range(0, gatherables.Count)];
    }
    private void SendDiseasedItem()
    {
        string diseasedItemName = diseasedItem.ItemNameKey;
        string json = JsonConvert.SerializeObject(diseasedItemName);
        FirebaseCommunicator.instance.SendObject(json, new string[] { diseasedReferenceName, FirebaseCommunicator.instance.FamilyId.ToString(), diseasedDate.ToString(dateFormat) }, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception.InnerException.Message);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Diseased item sent");
            }
        });
    }

    private void Update()
    {
        DateTime now = DateTime.Now;
        if (GotDiseased && (now - diseasedDate).Hours >= diseasedHourFrequency)
        {
            GetCurrentDate();
            GetDiseasedItem();
        }
    }

    private void GetCurrentDate()
    {
        diseasedDate = DateTime.Today;
        DateTime now = DateTime.Now;
        for (int i = 0; i <= 24 / diseasedHourFrequency; i++)
        {
            if (now.Hour < i * diseasedHourFrequency)
            {
                diseasedDate = diseasedDate.AddHours((i - 1) * diseasedHourFrequency);
                break;
            }
        }
    }
}
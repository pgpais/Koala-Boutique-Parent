using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoldManager : MonoBehaviour
{
    public static GoldManager instance;
    public static string goldReferenceName = "gold";
    public static string gemsReferenceName = "gems";
    public static UnityEvent<Item, int> ItemSoldEvent = new UnityEvent<Item, int>();
    public static UnityEvent<int> GoldChanged = new UnityEvent<int>();
    public static UnityEvent<int> GemChanged = new UnityEvent<int>();

    [field: SerializeField] public int CurrentGold { get; private set; } = 0;
    [field: SerializeField] public int CurrentGems { get; private set; } = 0;

    [SerializeField] bool testUpload = false;

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

        FirebaseCommunicator.LoggedIn.AddListener(GetCurrency);
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (testUpload)
        {
            testUpload = false;
            UploadGold();
        }
    }

    public void GetCurrency()
    {
        SetupGoldListener();
        SetupGemsListener();
        // GetGold();
        // GetGems();
    }

    private void GetGold()
    {
        FirebaseCommunicator.instance.GetObject(goldReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
                return;
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey got gold");
                string json = task.Result.GetRawJsonValue();
                Debug.Log("gold: " + json);
                if (string.IsNullOrEmpty(json))
                {
                    CurrentGold = 0;

                }
                else
                {
                    CurrentGold = int.Parse(json);
                }

                GoldChanged.Invoke(CurrentGold);
            }
        });
    }

    private void GetGems()
    {
        FirebaseCommunicator.instance.GetObject(gemsReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
                return;
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey got gems");
                string json = task.Result.GetRawJsonValue();
                Debug.Log("gems: " + json);
                if (string.IsNullOrEmpty(json))
                {
                    CurrentGems = 0;
                }
                else
                {
                    CurrentGems = int.Parse(json);
                }

                GemChanged.Invoke(CurrentGems);
            }
        });
    }

    void SetupGoldListener()
    {
        FirebaseCommunicator.instance.SetupListenForValueChangedEvents(goldReferenceName, (obj, args) =>
        {
            string json = args.Snapshot.GetRawJsonValue();
            if (string.IsNullOrEmpty(json))
            {
                CurrentGold = 0;
            }
            else
            {
                CurrentGold = int.Parse(json);
            }
            GoldChanged.Invoke(CurrentGold);
        });
    }

    void SetupGemsListener()
    {
        FirebaseCommunicator.instance.SetupListenForValueChangedEvents(gemsReferenceName, (obj, args) =>
        {
            string json = args.Snapshot.GetRawJsonValue();
            if (string.IsNullOrEmpty(json))
            {
                CurrentGems = 0;
            }
            else
            {
                CurrentGems = int.Parse(json);
            }
            GemChanged.Invoke(CurrentGems);
        });
    }

    internal void ModifyGold(int failedOfferingPenalty)
    {
        CurrentGold -= failedOfferingPenalty;
        UploadGold();
    }

    public bool HasEnoughGold(int amount)
    {
        return CurrentGold >= amount;
    }

    public bool HasEnoughGems(int amount)
    {
        return CurrentGems >= amount;
    }

    public void UploadGold()
    {
        FirebaseCommunicator.instance.SendObject(CurrentGold.ToString(), goldReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
                return;
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey updated gold");
            }

            GetCurrency();
        });
    }

    public void UploadGems()
    {
        FirebaseCommunicator.instance.SendObject(CurrentGems.ToString(), gemsReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
                return;
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey updated gem");
            }

            GetCurrency();
        });
    }

    public int SellItem(Item item, int amount)
    {
        int value = (item.GoldValue + MarketPrices.instance.GetCostModifierForItem(item.ItemNameKey)) * amount;
        CurrentGold += value;
        UploadGold();
        ItemSoldEvent.Invoke(item, amount);
        return value;
    }

    public int SellItem(Item item, int amount, float modifier)
    {
        int value = (int)((item.GoldValue + MarketPrices.instance.GetCostModifierForItem(item.ItemNameKey)) * modifier * amount);
        CurrentGold += value;
        UploadGold();
        ItemSoldEvent.Invoke(item, amount);
        return value;
    }

    public void BuyItem(Item item, int amount)
    {
        CurrentGold -= item.GoldValue * amount;
        UploadGold();
    }
    public void BuyUnlockable(Unlockable unlockable)
    {
        CurrentGems -= unlockable.GemCost;
        CurrentGold -= unlockable.GoldCost;
        UploadGems();
        UploadGold();
    }
}

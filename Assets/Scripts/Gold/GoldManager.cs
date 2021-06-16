using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoldManager : MonoBehaviour
{
    public static GoldManager instance;
    public static string referenceName = "gold";
    public static UnityEvent<int> GoldChanged = new UnityEvent<int>();

    [field: SerializeField] public int CurrentGold { get; private set; } = 0;

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

        FirebaseCommunicator.LoggedIn.AddListener(GetGold);
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

    public void GetGold()
    {
        FirebaseCommunicator.instance.GetObject(referenceName, (task) =>
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
                    GoldChanged.Invoke(CurrentGold);
                }
                else
                {
                    CurrentGold = int.Parse(json);
                    GoldChanged.Invoke(CurrentGold);
                }
            }
        });
    }

    public void UploadGold()
    {
        FirebaseCommunicator.instance.SendObject(CurrentGold.ToString(), referenceName, (task) =>
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

            GetGold();
        });
    }

    public void SellItem(Item item, int amount)
    {
        CurrentGold += (item.GoldValue + MarketPrices.instance.GetCostModifierForItem(item.ItemName)) * amount;
        UploadGold();
    }
}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class OfferingManager : MonoBehaviour
{
    public static UnityEvent<bool> OnOffering = new UnityEvent<bool>();

    public static string referenceName = "offering";
    public static OfferingManager Instance;

    Offering offering;

    const string dateFormat = "yyyyMMdd";


    public void SendItemsToOffering(List<Item> items)
    {
        if (offering.itemsToOffer == null)
        {
            Debug.LogError("OfferingManager: SendItemsToOffering: offering is null");
            return;
        }

        bool[] itemsChecked = new bool[offering.itemsToOffer.Count];

        foreach (Item item in items)
        {
            for (int i = 0; i < itemsChecked.Length && !itemsChecked[i]; i++)
            {
                if (item.ItemName == offering.itemsToOffer[i])
                {
                    itemsChecked[i] = true;
                }
            }
        }

        for (int i = 0; i < itemsChecked.Length; i++)
        {
            if (!itemsChecked[i])
            {
                Debug.Log("OfferingManager: SendItemsToOffering: item not found in offering: " + offering.itemsToOffer[i]);
                OnOffering.Invoke(false);
                return;
            }
        }

        Debug.Log("Good offer, king is pleased");
        OnOffering.Invoke(true);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        FirebaseCommunicator.LoggedIn.AddListener(OnLoggedIn);
    }

    void OnLoggedIn()
    {
        GetOffering();
    }

    void GetOffering()
    {
        FirebaseCommunicator.instance.GetObject(referenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("GetOffering: " + task.Exception.InnerException.Message);
            }
            else if (task.IsCompleted)
            {
                string json = task.Result.GetRawJsonValue();
                if (string.IsNullOrEmpty(json))
                {
                    Debug.Log("GetOffering: No data");
                    return;
                }
                else
                {
                    Debug.Log("GetOffering: " + json);
                    offering = JsonConvert.DeserializeObject<Offering>(json);
                    if (this.offering.HasExpired())
                    {
                        Debug.Log("GetOffering: Expired");
                        RemoveOffering();
                        return;
                    }
                }
            }
        });
    }

    void SendOffering(Offering offering)
    {
        string json = JsonConvert.SerializeObject(offering);
        FirebaseCommunicator.instance.SendObject(json, referenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error: " + task.Exception.InnerException.Message);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Success sending offering");
                this.offering = offering;
            }
        });
    }

    void RemoveOffering()
    {
        FirebaseCommunicator.instance.RemoveObject(referenceName, (task, obj) =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error: " + task.Exception.InnerException.Message);
                return;
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Success removing offering");
            }
        });
    }

    [Serializable]
    public struct Offering
    {
        public List<string> itemsToOffer;
        public string offerStartDate;
        public bool wasNotified;

        public Offering(List<string> itemsToOffer, string offerStartDate, bool wasNotified = false)
        {
            this.itemsToOffer = itemsToOffer;
            this.offerStartDate = offerStartDate;
            this.wasNotified = wasNotified;
        }

        internal bool HasExpired()
        {
            return DateTime.Today >= DateTime.ParseExact(offerStartDate, dateFormat, null).AddDays(2);
        }
    }
}
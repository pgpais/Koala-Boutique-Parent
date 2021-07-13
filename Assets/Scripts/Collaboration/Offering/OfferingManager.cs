using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class OfferingManager : MonoBehaviour
{
    public static string referenceName = "offering";
    public static OfferingManager Instance;

    Offering offering;

    const string dateFormat = "yyyyMMdd";

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
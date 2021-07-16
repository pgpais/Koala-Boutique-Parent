using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class SecretDoorManager : MonoBehaviour
{
    public static UnityEvent<string> OnCodeDecrypted = new UnityEvent<string>();

    public static string referenceName = "secretDoor";
    public static SecretDoorManager instance;

    public Item DoorKey => doorKey;
    [SerializeField] Item doorKey;
    const string dateFormat = "yyyyMMdd";
    DoorTime doorTime;

    internal string GetCode()
    {
        return doorTime.code;
    }

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

        FirebaseCommunicator.LoggedIn.AddListener(OnLoggedIn);
        ItemManager.OnGotItems.AddListener(CheckDoorItem);
    }

    private void OnLoggedIn()
    {
        GetDoorTime();
    }

    void GetDoorTime()
    {
        FirebaseCommunicator.instance.GetObject(referenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed getting door time");
                return;
            }
            else if (task.IsCompleted)
            {
                string json = task.Result.GetRawJsonValue();

                if (string.IsNullOrEmpty(json))
                {
                    doorTime = new DoorTime(null, null, false);
                }

                doorTime = JsonConvert.DeserializeObject<DoorTime>(json);

                DateTime requestDate = DateTime.ParseExact(doorTime.interactDate, dateFormat, null);
                //if today is after the date of the interact plus 2 days, remove request
                if (DateTime.Now >= requestDate.AddDays(2))
                {
                    DeleteRequest();
                }
            }
        });
    }

    //Delete Request
    void DeleteRequest()
    {
        doorTime.interactDate = null;
        SendDoorTime(doorTime);
    }

    void SendDoorTime(DoorTime doorTime)
    {
        //TODO: send item for processing
        string json = JsonConvert.SerializeObject(doorTime);
        FirebaseCommunicator.instance.SendObject(json, referenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to send door request");
                return;
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Door request sent");
            }
        });
    }

    void CheckDoorItem()
    {
        if (doorTime.unlocked)
        {
            return;
        }

        if (!ItemManager.instance.HasEnoughItem(doorKey.ItemName, 1))
        {
            Debug.Log("Don't have door key, creating one");
            ItemManager.instance.AddItem(doorKey.ItemName, 1, true);
        }
        else
        {
            Debug.Log("Already have door key");
        }
    }

    [System.Serializable]
    struct DoorTime
    {
        public string code;
        public string interactDate;
        public bool unlocked;

        public DoorTime(string code, string interactDate, bool unlocked = false)
        {
            this.code = code;
            this.interactDate = interactDate;
            this.unlocked = unlocked;
        }
    }

    internal void UnlockDoor(Item item)
    {
        int code = UnityEngine.Random.Range(1000, 10000);
        item.Description = "Code = " + code.ToString();

        doorTime.code = code.ToString();
        SendDoorTime(doorTime);

        OnCodeDecrypted.Invoke(doorTime.code);
    }
}
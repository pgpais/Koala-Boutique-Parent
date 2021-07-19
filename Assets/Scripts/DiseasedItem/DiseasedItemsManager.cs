using System;
using Newtonsoft.Json;
using UnityEngine;

public class DiseasedManager : MonoBehaviour
{
    private const string dateFormat = "yyyyMMdd HH";
    public static string referenceName = "diseasedItems";
    public static DiseasedManager instance;

    public string DiseasedItemName
    {
        get
        {
            if (requirement.Unlocked)
                return diseased.diseasedItemName;
            else
                return null;
        }
    }

    [SerializeField] Unlockable requirement;

    DiseasedTime diseased;

    DateTime currentDate;

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
    }

    void OnLoggedIn()
    {
        GetDiseasedItem();
    }

    void GetDiseasedItem()
    {
        DateTime now = DateTime.Now;
        for (int i = 24; i >= 0; i -= 8)
        {
            if (i <= now.Hour)
            {
                currentDate = new DateTime(now.Year, now.Month, now.Day, i, 00, 00);
                break;
            }
        }

        FirebaseCommunicator.instance.GetObject(new string[] { referenceName, FirebaseCommunicator.instance.FamilyId.ToString(), currentDate.ToString(dateFormat) }, (task) =>
              {
                  if (task.IsFaulted)
                  {
                      Debug.LogError("smth went wrong. " + task.Exception.ToString());
                      return;
                  }

                  if (task.IsCompleted)
                  {
                      string json = task.Result.GetRawJsonValue();
                      if (json == null)
                      {
                          Debug.Log("yey got diseased");
                          Debug.LogError("No Diseased Item today! creating a new one");
                          diseased = new DiseasedTime(null, currentDate.ToString(dateFormat));
                          return;
                      }

                      diseased = JsonConvert.DeserializeObject<DiseasedTime>(json);
                      Debug.Log("yey got diseased - " + diseased.diseasedItemName);

                  }
              });
    }

    private void Update()
    {
        if (!requirement.Unlocked)
        {
            return;
        }

        DateTime now = DateTime.Now;
        if (now.Hour >= currentDate.Hour + 8 || now.DayOfYear > currentDate.DayOfYear)
        {
            currentDate = currentDate.AddHours(8);
            GetDiseasedItem();
        }

        if (string.IsNullOrEmpty(diseased.diseasedItemName))
        {
            CreateDiseasedItem(currentDate);
            diseased = new DiseasedTime("null", currentDate.ToString(dateFormat));
        }
    }

    public void CreateDiseasedItem(DateTime diseaseDate)
    {
        var item = ItemManager.instance.itemsData.GetRandomItem((item) => item.Type == Item.ItemType.Gatherable && item.Unlocked);
        DiseasedTime newDiseased = new DiseasedTime(item.ItemName, diseaseDate.ToString(dateFormat));

        string json = JsonConvert.SerializeObject(newDiseased);

        FirebaseCommunicator.instance.SendObject(json, new string[] { referenceName, FirebaseCommunicator.instance.FamilyId.ToString(), diseaseDate.ToString(dateFormat) }, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to create new diseased item! Message:" + task.Exception.Message);
                diseased = new DiseasedTime(null, currentDate.ToString(dateFormat));
                return;
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Created new Diseased item!");
                diseased = newDiseased;
            }
        });
    }

    [System.Serializable]
    struct DiseasedTime
    {
        public string diseasedItemName;
        public string diseaseDate;

        public DiseasedTime(string diseasedItemName, string diseaseDate)
        {
            this.diseasedItemName = diseasedItemName;
            this.diseaseDate = diseaseDate;
        }
    }
}
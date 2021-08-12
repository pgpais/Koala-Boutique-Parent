using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class UnlockablesManager : MonoBehaviour
{
    public static string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    public static string unlocksReferenceName = "techs";
    public static string difficultyReferenceName = "difficulty";
    public static string unlocksLogReferenceName = "unlocksLog";
    public static UnityEvent OnGotUnlockables = new UnityEvent();
    public static UnlockablesManager instance;

    public bool GotUnlockables { get; private set; }

    [SerializeField] UnlockablesList unlockablesData;

    public Dictionary<string, Unlockable> Unlockables => unlockables;
    Dictionary<string, Unlockable> unlockables;

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

        unlockables = new Dictionary<string, Unlockable>();

        foreach (var unlockable in unlockablesData.unlockables)
        {
            unlockable.InitializeEvent();
            unlockables.Add(unlockable.UnlockableKeyName, unlockable);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FirebaseCommunicator.LoggedIn.AddListener(OnLoggedIn);
    }

    private void OnLoggedIn()
    {
        GetUnlockedUnlockables();
    }

    void OnLogout()
    {
        foreach (Unlockable unlockable in unlockables.Values)
        {
            unlockable.Reset();
        }
    }

    void GetUnlockedUnlockables()
    {
        FirebaseCommunicator.instance.GetObject(unlocksReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey got unlocks");
                string json = task.Result.GetRawJsonValue();
                if (string.IsNullOrEmpty(json))
                {
                    Debug.Log("no unlocks");
                }
                else
                {
                    string[] unlockedNames = JsonHelper.DeserializeArray<string>(json);
                    foreach (var unlockableName in unlockedNames)
                    {
                        unlockables[unlockableName].Unlock();
                    }
                }
                OnGotUnlockables.Invoke();
                GotUnlockables = true;
            }
        });
    }

    internal bool Unlock(Unlockable unlockable)
    {
        foreach (var requirement in unlockable.Requirements)
        {
            if (!requirement.Unlocked)
            {
                LogsManager.SendLogDirectly(new Log(
                    LogType.Unlock,
                    new Dictionary<string, string>(){
                        { "unlockable", unlockable.UnlockableKeyName },
                        {"successful", false.ToString()} //beautiful
                    }
                ));

                return false;
            }
        }

        foreach (var cost in unlockable.ItemCost)
        {
            if (!ItemManager.instance.HasEnoughItem(cost.Key.ItemNameKey, cost.Value))
            {
                LogsManager.SendLogDirectly(new Log(
                    LogType.Unlock,
                    new Dictionary<string, string>(){
                        { "unlockable", unlockable.UnlockableKeyName },
                        {"successful", false.ToString()} //beautiful
                    }
                ));

                return false;
            }
        }

        if (!GoldManager.instance.HasEnoughGems(unlockable.GemCost) || !GoldManager.instance.HasEnoughGold(unlockable.GoldCost))
        {
            LogsManager.SendLogDirectly(new Log(
                LogType.Unlock,
                new Dictionary<string, string>(){
                    { "unlockable", unlockable.UnlockableKeyName },
                    {"successful", false.ToString()} //beautiful
                }
            ));

            return false;
        }

        foreach (var cost in unlockable.ItemCost)
        {
            ItemManager.instance.RemoveItem(cost.Key.ItemNameKey, cost.Value);
        }
        GoldManager.instance.BuyUnlockable(unlockable);

        UnlockLog log = new UnlockLog(unlockable.UnlockableKeyName, DateTime.Now);
        SaveUnlockLog(log);

        LogsManager.SendLogDirectly(new Log(
            LogType.Unlock,
            new Dictionary<string, string>(){
                { "unlockable", unlockable.UnlockableKeyName },
                {"successful", true.ToString()} //beautiful
            }
        ));

        unlockable.Unlock();
        ModifyDifficulty(unlockable.DifficultyModifier);
        SaveUnlockOnCloud();
        return true;
    }

    void SaveUnlockOnCloud()
    {
        List<string> list = unlockables.Where(u => u.Value.Unlocked).Select(u => u.Value.UnlockableKeyName).ToList();

        string json = "[";
        foreach (var item in list)
        {
            json += "\"" + item + "\"" + ",";
        }
        json = json.Substring(0, json.Length - 1);
        json += "]";

        Debug.Log(json);

        FirebaseCommunicator.instance.SendObject(json, unlocksReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey unlocked sync");
            }
        });
    }

    void SaveUnlockLog(UnlockLog log)
    {
        string json = JsonConvert.SerializeObject(log);
        string key = FirebaseCommunicator.instance.Push(unlocksLogReferenceName);
        FirebaseCommunicator.instance.SendObject(json, new String[] { unlocksLogReferenceName, FirebaseCommunicator.instance.FamilyId.ToString(), key }, (task) =>
          {
              if (task.IsFaulted)
              {
                  Debug.LogError("smth went wrong. " + task.Exception.ToString());
              }
              if (task.IsCompleted)
              {
                  Debug.Log("yey added new update log");
              }
          });
    }

    void ModifyDifficulty(int amount)
    {
        FirebaseCommunicator.instance.GetObject(difficultyReferenceName, (task) =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("smth went wrong. " + task.Exception.ToString());
                    return;
                }

                if (task.IsCompleted)
                {
                    Debug.Log("yey got difficulty");
                    int difficulty;

                    string json = task.Result.GetRawJsonValue();
                    if (json == null)
                    {
                        Debug.LogError("No difficulty found!");
                        difficulty = 0;
                        return;
                    }

                    difficulty = JsonConvert.DeserializeObject<int>(json);
                    difficulty += amount;

                    json = JsonConvert.SerializeObject(difficulty);
                    FirebaseCommunicator.instance.SendObject(json, difficultyReferenceName, (task) =>
                    {
                        if (task.IsFaulted)
                        {
                            Debug.LogError("smth went wrong. " + task.Exception.ToString());
                            return;
                        }

                        if (task.IsCompleted)
                        {
                            Debug.Log("CLOUD: Updated Difficulty");
                        }
                    });
                }
            });
    }

    public bool IsUnlockableUnlocked(string unlockableName)
    {
        if (!unlockables.ContainsKey(unlockableName))
        {
            Debug.LogError("Unlockable with name " + unlockableName + " does not exist!");

            return false;
        }

        return unlockables[unlockableName].Unlocked;
    }
}

struct UnlockLog
{
    public string UnlockableName { get; private set; }
    public string UnlockDate { get; private set; }

    public DateTime UnlockDateTime()
    {
        return DateTime.ParseExact(UnlockDate, UnlockablesManager.dateTimeFormat, null);
    }

    public UnlockLog(string unlockableName, string unlockDate)
    {
        UnlockableName = unlockableName;
        UnlockDate = unlockDate;
    }

    public UnlockLog(string unlockableName, DateTime unlockDate)
    {
        UnlockableName = unlockableName;
        UnlockDate = unlockDate.ToString(UnlockablesManager.dateTimeFormat);
    }
}

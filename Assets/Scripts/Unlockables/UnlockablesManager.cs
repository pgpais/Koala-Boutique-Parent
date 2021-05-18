using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnlockablesManager : MonoBehaviour
{
    public static UnlockablesManager instance;

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
            unlockables.Add(unlockable.UnlockableName, unlockable);
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

    void GetUnlockedUnlockables()
    {
        FirebaseCommunicator.instance.GetObject("techs", (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey got unlocks");
                string[] unlockedNames = JsonHelper.DeserializeArray<string>(task.Result.GetRawJsonValue());
                foreach (var unlockableName in unlockedNames)
                {
                    unlockables[unlockableName].Unlock();
                }
            }
        });
    }

    internal void Unlock(Unlockable unlockable)
    {
        bool success = true;

        foreach (var requirement in unlockable.Requirements)
        {
            if (!requirement.Unlocked)
                success = false;
        }

        foreach (var cost in unlockable.Cost)
        {
            if (!ItemManager.instance.HasEnoughItem(cost.Key.ItemName, cost.Value))
            {
                success = false;
            }
        }

        if (success)
        {
            foreach (var cost in unlockable.Cost)
            {
                ItemManager.instance.RemoveItem(cost.Key.ItemName, cost.Value);
            }

            unlockable.Unlock();
            SaveUnlockOnCloud();
        }
    }

    void SaveUnlockOnCloud()
    {
        List<string> list = unlockables.Where(u => u.Value.Unlocked).Select(u => u.Value.UnlockableName).ToList();

        string json = "[";
        foreach (var item in list)
        {
            json += "\"" + item + "\"" + ",";
        }
        json = json.Substring(0, json.Length - 1);
        json += "]";

        Debug.Log(json);

        FirebaseCommunicator.instance.SendObject(json, "techs", (task) =>
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

    // Update is called once per frame
    void Update()
    {

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

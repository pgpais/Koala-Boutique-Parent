using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class MissionManager : MonoBehaviour
{
    public static string difficultyReferenceName = "difficulty";
    public static string abundantGatherableReferenceName = "abundantGatherable";
    public static UnityEvent<int> OnGotDifficulty = new UnityEvent<int>();
    public static UnityEvent<Item> OnGotAbundantGatherable = new UnityEvent<Item>();
    public static MissionManager instance;

    public bool GotDifficulty { get; private set; } = false;
    public bool GotAbundantGatherable { get; private set; } = false;
    public Item AbundantGatherable => abundantGatherable;
    public int Difficulty => difficulty;

    private int difficulty;
    private Item abundantGatherable;


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

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnLoggedIn()
    {
        SetupDifficultyListener();
        SetupAbundantGatherableListener();
        // GetDifficulty();
        // GetAbundantGatherable();
    }

    private void SetupDifficultyListener()
    {
        FirebaseCommunicator.instance.SetupListenForValueChangedEvents(difficultyReferenceName, (obj, args) =>
        {
            string json = args.Snapshot.GetRawJsonValue();
            if (string.IsNullOrEmpty(json))
            {
                difficulty = 0;
                Debug.LogError("Failed to get difficulty from Firebase: empty json");
            }
            else
            {
                difficulty = JsonConvert.DeserializeObject<int>(json);
                OnGotDifficulty.Invoke(difficulty);
                GotDifficulty = true;
                Debug.Log("Difficulty: " + difficulty);
            }
        });
    }

    void GetDifficulty()
    {
        FirebaseCommunicator.instance.GetObject(difficultyReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to get difficulty from Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                string json = task.Result.GetRawJsonValue();
                if (string.IsNullOrEmpty(json))
                {
                    difficulty = -1;
                    Debug.LogError("Failed to get difficulty from Firebase: empty json");
                }
                else
                {
                    difficulty = JsonConvert.DeserializeObject<int>(json);
                    OnGotDifficulty.Invoke(difficulty);
                    GotDifficulty = true;
                    Debug.Log("Difficulty: " + difficulty);
                }
            }
        });
    }

    private void SetupAbundantGatherableListener()
    {
        FirebaseCommunicator.instance.SetupListenForValueChangedEvents(abundantGatherableReferenceName, (obj, args) =>
        {
            string json = args.Snapshot.GetRawJsonValue();

            if (string.IsNullOrEmpty(json))
            {
                abundantGatherable = ItemManager.instance.itemsData.GetItemByName("Basic Mushroom");
                Debug.LogError("Failed to get abundant gatherable from Firebase: empty json");
            }
            else
            {
                string itemName = JsonConvert.DeserializeObject<string>(json);
                abundantGatherable = ItemManager.instance.itemsData.GetItemByName(itemName);
                Debug.Log("Abundant Gatherable: " + abundantGatherable);
            }
            OnGotAbundantGatherable.Invoke(abundantGatherable);
            GotAbundantGatherable = true;
        });
    }

    void GetAbundantGatherable()
    {
        FirebaseCommunicator.instance.GetObject(abundantGatherableReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to get abundant gatherable from Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                string json = task.Result.GetRawJsonValue();
                if (string.IsNullOrEmpty(json))
                {
                    abundantGatherable = ItemManager.instance.itemsData.GetItemByName("Basic Mushroom");
                    Debug.LogError("Failed to get abundant gatherable from Firebase: empty json");
                }
                else
                {
                    string itemName = JsonConvert.DeserializeObject<string>(json);
                    abundantGatherable = ItemManager.instance.itemsData.GetItemByName(itemName);
                    Debug.Log("Abundant Gatherable: " + abundantGatherable);
                }
                OnGotAbundantGatherable.Invoke(abundantGatherable);
                GotAbundantGatherable = true;
            }
        });
    }

    public void SetDifficulty(MissionDifficulty difficulty)
    {
        LogsManager.SendLogDirectly(new Log(
            LogType.DifficultySelected,
            new Dictionary<string, string>(){
                {"Difficulty", difficulty.ToString()}
            }
        ));

        switch (difficulty)
        {
            case MissionDifficulty.Easy:
                this.difficulty = 10;
                break;
            case MissionDifficulty.Medium:
                this.difficulty = 20;
                break;
            case MissionDifficulty.Hard:
                this.difficulty = 30;
                break;
            default:
                this.difficulty = -1;
                break;
        }

        SetDifficultyCloud();
    }

    public void SetDifficulty(int difficulty)
    {
        this.difficulty = difficulty;

        SetDifficultyCloud();
    }

    void SetDifficultyCloud()
    {
        string json = JsonConvert.SerializeObject(difficulty);
        FirebaseCommunicator.instance.SendObject(json, difficultyReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to set difficulty to Firebase: " + task.Exception);
            }
        });
    }

    public void SetAbundantGatherable(Item abundantGatherable)
    {
        this.abundantGatherable = abundantGatherable;

        SetAbundantGatherableCloud();
    }

    void SetAbundantGatherableCloud()
    {
        string json = JsonConvert.SerializeObject(abundantGatherable.name);
        FirebaseCommunicator.instance.SendObject(json, abundantGatherableReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to set abundant gatherable to Firebase: " + task.Exception);
            }
        });
    }
}

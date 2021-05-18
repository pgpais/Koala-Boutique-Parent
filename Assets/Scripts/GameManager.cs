using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static UnityEvent<Mission> MissionGenerated = new UnityEvent<Mission>();
    public static UnityEvent<Mission> MissonCompleted = new UnityEvent<Mission>();

    public Mission CurrentMission { get; private set; }

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
    }

    // Start is called before the first frame update
    void Start()
    {
        FirebaseCommunicator.GameStarted.AddListener(OnGameStarted);
    }

    void OnGameStarted()
    {
        GetMissionFromCloud();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateMission()
    {
        CurrentMission = new Mission();

        SaveMission(CurrentMission);
    }

    void GetMissionFromCloud()
    {
        FirebaseCommunicator.instance.GetObject("missions", (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey got mission");
                CurrentMission = JsonConvert.DeserializeObject<Mission>(task.Result.GetRawJsonValue());
                MissionGenerated.Invoke(CurrentMission);
            }
        });
    }

    private void SaveMission(Mission mission)
    {
        FirebaseCommunicator.instance.SendObject(JsonUtility.ToJson(mission), Mission.firebaseReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong saving mission");
                MissionGenerated.Invoke(null);
            }

            if (task.IsCanceled)
            {
                Debug.LogError("saving mission canceled");
                MissionGenerated.Invoke(null);
            }

            if (task.IsCompleted)
            {
                Debug.Log("Successfully saved task!");
                MissionGenerated.Invoke(CurrentMission);
                FirebaseCommunicator.instance.SetupListenForValueChangedEvents(new string[] { Mission.firebaseReferenceName, FirebaseCommunicator.instance.FamilyId.ToString(), "successfulRun" }, OnMissionComplete);
            }
        });
    }

    private void OnMissionComplete(object sender, ValueChangedEventArgs args)
    {
        CurrentMission.successfulRun = args.Snapshot.GetRawJsonValue() == "true";
        GameManager.MissonCompleted.Invoke(CurrentMission);

        if (CurrentMission.successfulRun)
            FirebaseCommunicator.instance.RemoveValueChangedListener(new string[] { Mission.firebaseReferenceName, FirebaseCommunicator.instance.FamilyId.ToString(), "successfulRun" }, OnMissionComplete);
    }
}

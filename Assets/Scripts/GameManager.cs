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

    public Mission GeneratedMission { get; private set; }

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
        GeneratedMission = new Mission();

        SaveMission(GeneratedMission);
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
                GeneratedMission = JsonConvert.DeserializeObject<Mission>(task.Result.GetRawJsonValue());
                MissionGenerated.Invoke(GeneratedMission);
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
                MissionGenerated.Invoke(GeneratedMission);
                FirebaseCommunicator.instance.SetupListenForValueChangedEvents(new string[] { Mission.firebaseReferenceName, FirebaseCommunicator.instance.FamilyId.ToString(), "successfulRun" }, OnMissionComplete);
            }
        });
    }

    private void OnMissionComplete(object sender, ValueChangedEventArgs args)
    {
        GeneratedMission.successfulRun = args.Snapshot.GetRawJsonValue() == "true";
        GameManager.MissonCompleted.Invoke(GeneratedMission);

        if (GeneratedMission.successfulRun)
            FirebaseCommunicator.instance.RemoveValueChangedListener(new string[] { Mission.firebaseReferenceName, FirebaseCommunicator.instance.FamilyId.ToString(), "successfulRun" }, OnMissionComplete);
    }
}

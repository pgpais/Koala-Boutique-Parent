using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;

    public static UnityEvent<Mission> MissionUpdated = new UnityEvent<Mission>();

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
        FirebaseCommunicator.LoggedIn.AddListener(OnLoggedIN);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnLoggedIN()
    {
        Debug.Log("Logged in missionManager");
        GetMissionFromCloud();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateMission(MissionZone zone, MissionDifficulty difficulty)
    {
        CurrentMission = new Mission(zone, difficulty);


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
                MissionUpdated.Invoke(CurrentMission);
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
                MissionUpdated.Invoke(null);
            }

            if (task.IsCanceled)
            {
                Debug.LogError("saving mission canceled");
                MissionUpdated.Invoke(null);
            }

            if (task.IsCompleted)
            {
                Debug.Log("Successfully saved Mission!");
                MissionUpdated.Invoke(CurrentMission);
                FirebaseCommunicator.instance.SetupListenForValueChangedEvents(new string[] { Mission.firebaseReferenceName, FirebaseCommunicator.instance.FamilyId.ToString(), "successfulRun" }, OnMissionComplete);
            }
        });
    }

    private void OnMissionComplete(object sender, ValueChangedEventArgs args)
    {
        CurrentMission.completed = args.Snapshot.GetRawJsonValue() == "true";
        MissionManager.MissionUpdated.Invoke(CurrentMission);

        if (CurrentMission.completed)
            FirebaseCommunicator.instance.RemoveValueChangedListener(new string[] { Mission.firebaseReferenceName, FirebaseCommunicator.instance.FamilyId.ToString(), "successfulRun" }, OnMissionComplete);
    }
}

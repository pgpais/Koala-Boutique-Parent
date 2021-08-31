using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

public class LogsManager
{
    private const string FirebaseReferenceName = "logs";

    public static LogsManager instance = new LogsManager();

    private List<Log> logs;


    public LogsManager()
    {
        if (instance == null)
        {
            instance = this;
            logs = new List<Log>();
        }
        else
        {
            Debug.LogError("LogsManager already initialized");
        }
    }

    public void AddLog(Log log)
    {
        logs.Add(log);
    }

    public void ClearLogs()
    {
        logs.Clear();
    }

    public void SaveLogs()
    {
        foreach (var log in logs)
        {
            SendLogDirectly(log);
        }

        ClearLogs();
    }

    public static void SendLogDirectly(Log log)
    {
        string json = JsonConvert.SerializeObject(log);
        FirebaseCommunicator.instance.SendObject(json, new string[] { FirebaseReferenceName, FirebaseCommunicator.instance.FamilyId.ToString(), log.Timestamp }, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error with ID: " + FirebaseCommunicator.instance.FamilyId.ToString() + "sending log " + log.Type + ": " + task.Exception.InnerException.Message);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Log sent");
            }
        });
    }
}

public class Log
{
    private const string dateFormat = "yyyy-MM-dd HH:mm:ss:ffff";
    public string platform = "Mobile";

    [JsonConverter(typeof(StringEnumConverter))]
    public LogType Type { get; private set; }
    public Dictionary<string, string> Data { get; private set; }
    public string Timestamp { get; private set; }

    public Log(LogType type, Dictionary<string, string> data, DateTime time)
    {
        this.Type = type;
        this.Data = data;
        this.Timestamp = time.ToString(dateFormat);
    }

    public Log(LogType type, Dictionary<string, string> data)
    {
        this.Type = type;
        this.Data = data;
        this.Timestamp = DateTime.Now.ToString(dateFormat);
    }
    public Log(LogType type)
    {
        this.Type = type;
        this.Data = null;
        this.Timestamp = DateTime.Now.ToString(dateFormat);
    }
}

public enum LogType
{
    MissionStart,
    MissionSuccess,
    MissionFail,
    Death,
    EnemyKilled,
    DamageTaken,
    AttackPerformed,
    MushroomCollected,
    DiseasedItemCollected,
    LootCollected,
    BuffCollected,
    RoomExplored,
    RoomEntered,
    GotOracleInfo,
    OracleInteracted,
    KingOfferingReceived,
    KingOfferingChecked,
    KingOfferingSuccess,
    KingOfferingFail,
    SecretDoorInteracted,
    CorrectCodeInserted,
    WrongCodeInserted,
    HealingReceived,
    LootChestCollected,
    ItemProcessStarted,
    ItemProcessFinished,
    ProcessBoost,
    ItemSoldRetail,
    ItemSoldMarket,
    AbundantItemSelected,
    Unlock,
    AdventurerQuestCreated,
    AdventurerQuestChecked,
    AdventurerQuestSuccess,
    ManagerQuestCreated,
    ManagerQuestChecked,
    ManagerQuestSuccess,
    LoggedIn,
    LoggedOut,
    GameQuit,
    GameStarted,
    DifficultySelected,
    ClassSelected,
    NotebookInteracted,
    AdventurerNewsSeen,
    ManagerNewsSeen,
    SecretCodeChecked,
    MobileTabSwitched,
    UnlocksTabSwitched,
    Paused,
    SceneLoaded,
    SceneUnloaded,
    EncryptedKeyProcessed
}

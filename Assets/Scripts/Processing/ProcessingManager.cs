using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class ProcessingManager : MonoBehaviour
{
    public static ProcessingManager instance;
    public static string firebaseReferenceName = "processes";

    public static UnityEvent GotProcesses = new UnityEvent();
    public static UnityEvent<string, Process> ProcessCreated = new UnityEvent<string, Process>();

    public List<Process> InProcess => inProcess;
    public float BoostCooldown => boostCooldown;
    public float NextBoostTime => nextBoostTime;

    [SerializeField] NewProcessMenu newProcessMenu;
    [SerializeField] bool startProcessing;
    [SerializeField] float boostCooldown = 5f;

    float nextBoostTime;

    List<Process> inProcess;


    private void Awake()
    {
        inProcess = new List<Process>();
        ItemManager.OnGotItems.AddListener(Initialize);
        FirebaseCommunicator.LoggedOut.AddListener(OnLogout);

        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        nextBoostTime = Time.time + boostCooldown;
    }

    // Start is called before the first frame update
    void Initialize()
    {
        ItemManager.OnGotItems.RemoveListener(Initialize);
        Debug.Log(JsonConvert.SerializeObject(DateTime.UtcNow));

        FirebaseCommunicator.instance.GetObject(firebaseReferenceName, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey got processes");
                // Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(task.Result.GetRawJsonValue());
                Dictionary<string, object> dictionary = task.Result.Value as Dictionary<string, object>;
                // Dictionary<string, Process> dictionary = JsonConvert.DeserializeObject<Dictionary<string, Process>>(task.Result.GetRawJsonValue());

                foreach (var key in dictionary.Keys)
                {
                    // Debug.Log(dictionary[key]);
                    string processJSON = JsonConvert.SerializeObject(dictionary[key]);

                    try
                    {
                        Process process = JsonConvert.DeserializeObject<Process>(processJSON);
                        AddProcessToList(process);
                        process.Key = key;
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex, this);
                    }
                }

                GotProcesses.Invoke();
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (startProcessing)
        {
            StartProcessing("SlowProcessLootable1", 1);
            startProcessing = false;
        }
    }

    public void ShowNewProcessMenu(string itemName, int minAmount, int maxAmount)
    {
        newProcessMenu.Init(itemName, minAmount, maxAmount);
    }

    public void StartProcessing(string itemNameKey, int amount)
    {
        if (ItemManager.instance.HasEnoughItem(itemNameKey, amount))
        {
            // remove from global inventory
            ItemManager.instance.RemoveItem(itemNameKey, amount);

            // create Process
            CreateProcess(itemNameKey, amount);

            LogsManager.SendLogDirectly(new Log(
                LogType.ItemProcessStarted,
                new Dictionary<string, string>(){
                    {"itemName", itemNameKey},
                    {"amount", amount.ToString()}
                }
            ));
        }
        else
        {
            Debug.LogError("Not enough items");
            int remainingAmount = ItemManager.instance.itemQuantity[itemNameKey];
            ItemManager.instance.RemoveItem(itemNameKey, remainingAmount);

            CreateProcess(itemNameKey, remainingAmount);

            LogsManager.SendLogDirectly(new Log(
                LogType.ItemProcessStarted,
                new Dictionary<string, string>(){
                    {"itemName", itemNameKey},
                    {"amount", remainingAmount.ToString()}
                }
            ));
        }
        LogsManager.SendLogDirectly(new Log(
            LogType.ItemProcessStarted,
            new Dictionary<string, string>(){
                {"itemName", itemNameKey},
                {"amount", "0"}
            }
        ));
    }

    public void BoostProcesses()
    {
        LogsManager.SendLogDirectly(new Log(
            LogType.ProcessBoost,
            new Dictionary<string, string>(){
                {"successful", CanBoost().ToString()}
            }
        ));

        if (CanBoost())
        {
            foreach (var process in inProcess)
            {
                process.Boost();
            }

            nextBoostTime = Time.time + boostCooldown;
        }
    }

    public bool CanBoost() => Time.time >= nextBoostTime;

    void CreateProcess(string itemName, int amount)
    {
        var item = ItemManager.instance.itemsData.GetItemByName(itemName);
        string key = FirebaseCommunicator.instance.Push(firebaseReferenceName);

        Process process = new Process(item, amount);

        string json = JsonConvert.SerializeObject(process);

        Debug.Log(json);

        FirebaseCommunicator.instance.SendObject(json, firebaseReferenceName, key, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("CLOUD: Failed sending new process");
                return;
            }

            if (task.IsCompleted)
            {
                Debug.Log("CLOUD: sent new process");
                AddProcessToList(process);
                process.Key = key;
            }
        });

    }

    void AddProcessToList(Process process)
    {
        inProcess.Add(process);

        process.ProcessFinished.AddListener(() => OnProcessFinished(process));

        Debug.Log(JsonConvert.SerializeObject(process));
        ProcessCreated.Invoke(process.ProcessingItemName, process);
    }

    void OnProcessFinished(Process process)
    {
        LogsManager.SendLogDirectly(new Log(
            LogType.ItemProcessFinished,
            new Dictionary<string, string>(){
                {"itemName", process.ProcessingItemName},
                {"amount", process.ProcessAmount.ToString()}
            }
        ));

        Item processItem = ItemManager.instance.itemsData.GetItemByName(process.ProcessingItemName);
        if (processItem.ItemNameKey == SecretDoorManager.instance.DoorKey.ItemNameKey)
        {
            Debug.Log("Unlocking secret code!");
            SecretDoorManager.instance.UnlockDoor(processItem.ProcessResult);
        }
        else
        {
            Debug.Log("Adding process result to items!");
            ItemManager.instance.AddItem(processItem.ProcessResult.ItemNameKey, processItem.ProcessResultAmount * process.ProcessAmount, true);
        }

        inProcess.Remove(process);

        FirebaseCommunicator.instance.RemoveObject(firebaseReferenceName, process.Key, (task, obj) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey removed " + process.Key + " from processes");
            }
        });
    }

    void OnLogout()
    {
        inProcess.Clear();
    }
}

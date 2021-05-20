using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class ProcessingManager : MonoBehaviour
{
    static string firebaseReferenceName = "processes";

    public static UnityEvent<string, Process> ProcessCreated = new UnityEvent<string, Process>();

    [SerializeField] bool startProcessing;

    List<Process> inProcess;

    private void Awake()
    {
        inProcess = new List<Process>();
        FirebaseCommunicator.LoggedIn.AddListener(Initialize);
    }

    // Start is called before the first frame update
    void Initialize()
    {
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


        for (var i = 0; i < inProcess.Count; i++)
        {
            inProcess[i].DoTick();
        }
    }

    public void StartProcessing(string itemName, int amount)
    {
        if (ItemManager.instance.HasEnoughItem(itemName, amount))
        {
            // remove from global inventory
            ItemManager.instance.RemoveItem(itemName, amount);

            // create Process
            CreateProcess(itemName, amount);
        }
    }

    void CreateProcess(string itemName, int amount)
    {
        var item = ItemManager.instance.itemsData.GetItemByName(itemName);
        string key = FirebaseCommunicator.instance.Push(firebaseReferenceName);

        Process process = new Process(item.ProcessDuration, amount, item.BoostTimeAmount, item.BoostCooldown, item.ProcessResult.ItemName, item.ProcessResultAmount, itemName, key);

        FirebaseCommunicator.instance.SendObject(JsonConvert.SerializeObject(process), firebaseReferenceName, key, (task) =>
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
        ProcessCreated.Invoke(process.ProcessedItemName, process);
    }

    void OnProcessFinished(Process process)
    {
        ItemManager.instance.AddItem(process.ResultItemName, process.ResultItemAmount, true);

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
}

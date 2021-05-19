using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProcessingManager : MonoBehaviour
{
    static string firebaseReferenceName = "processing";

    public static UnityEvent<string, Process> ProcessCreated = new UnityEvent<string, Process>();

    Dictionary<string, Process> inProcess;

    private void Awake()
    {
        inProcess = new Dictionary<string, Process>();
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateProcess("Lootable1", 5);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var process in inProcess.Values)
        {
            process.DoTick();
        }
    }

    public void StartProcessing(string itemName, int amount)
    {
        if (ItemManager.instance.HasEnoughItem(itemName, amount))
        {
            // remove from global inventory

            // create Process
        }
    }

    public void CreateProcess(string itemName, int amount)
    {
        Process process = new Process(5, amount);

        // TODO: Set process time in itemData
        inProcess.Add(itemName, process);
        Debug.Log("Created process that will take " + inProcess[itemName].TimeLeft.ToString() + " seconds");

        // TODO: create UI
        ProcessCreated.Invoke(itemName, process);
    }
}

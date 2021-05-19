using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProcessingManager : MonoBehaviour
{
    static string firebaseReferenceName = "processing";

    public static UnityEvent<string, Process> ProcessCreated = new UnityEvent<string, Process>();

    [SerializeField] bool startProcessing;

    List<Process> inProcess;

    private void Awake()
    {
        inProcess = new List<Process>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (startProcessing)
        {
            StartProcessing("Lootable1", 1);
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

        Process process = new Process(item.ProcessDuration, amount, item.BoostTimeAmount, item.BoostCooldown, item.ProcessResult.ItemName, item.ProcessResultAmount);

        inProcess.Add(process);
        Debug.Log("Created process that will take " + process.TimeLeft.ToString() + " seconds");

        process.ProcessFinished.AddListener(() => OnProcessFinished(process));

        ProcessCreated.Invoke(itemName, process);
    }

    void OnProcessFinished(Process process)
    {
        ItemManager.instance.AddItem(process.ResultItemName, process.ResultItemAmount, true);

        inProcess.Remove(process);
    }
}

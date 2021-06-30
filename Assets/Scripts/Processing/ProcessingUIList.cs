using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessingUIList : MonoBehaviour
{
    [SerializeField] ItemProcessingUI processUIPrefab;
    [SerializeField] Transform processList;

    private void Awake()
    {
        foreach (Process process in ProcessingManager.instance.InProcess)
        {
            AddNewProcessUI(process.ProcessingItemName, process);
        }

        ProcessingManager.ProcessCreated.AddListener(AddNewProcessUI);
    }

    void AddNewProcessUI(string itemName, Process process)
    {
        Instantiate(processUIPrefab, processList).Init(itemName, process);
    }
}

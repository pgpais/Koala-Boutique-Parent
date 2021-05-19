using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessingUIList : MonoBehaviour
{
    [SerializeField] ItemProcessingUI processUIPrefab;
    [SerializeField] Transform processList;

    private void Awake()
    {
        ProcessingManager.ProcessCreated.AddListener(AddNewProcessUI);
    }

    void AddNewProcessUI(string itemName, Process process)
    {
        Instantiate(processUIPrefab, processList).Init(itemName, process);
    }
}

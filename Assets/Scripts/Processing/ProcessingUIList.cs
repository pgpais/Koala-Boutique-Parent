using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcessingUIList : MonoBehaviour
{
    [SerializeField] ItemProcessingUI processUIPrefab;
    [SerializeField] Transform processList;
    [SerializeField] Button boostButton;

    private void Start()
    {
        boostButton.onClick.AddListener(BoostProcesses);
    }

    private void Update()
    {
        float boostRatio = (ProcessingManager.instance.NextBoostTime - Time.time) / ProcessingManager.instance.BoostCooldown;
        boostButton.image.color = Color.Lerp(Color.red, Color.white, boostRatio);
    }

    private void OnEnable()
    {
        foreach (Process process in ProcessingManager.instance.InProcess)
        {
            AddNewProcessUI(process.ProcessingItemName, process);
        }
    }

    private void OnDisable()
    {
        foreach (Transform process in processList)
        {
            Destroy(process.gameObject);
        }
    }

    void AddNewProcessUI(string itemName, Process process)
    {
        Instantiate(processUIPrefab, processList).Init(itemName, process);
    }

    void BoostProcesses()
    {
        ProcessingManager.instance.BoostProcesses();
    }
}

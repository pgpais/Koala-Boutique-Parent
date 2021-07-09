using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcessingUIList : MonoBehaviour
{
    [SerializeField] ItemProcessingUI processUIPrefab;
    [SerializeField] Transform processList;
    [SerializeField] Button boostButton;

    private void Awake()
    {

        boostButton.onClick.AddListener(BoostProcesses);

        ProcessingManager.GotProcesses.AddListener(() =>
        {
            foreach (Process process in ProcessingManager.instance.InProcess)
            {
                AddNewProcessUI(process.ProcessingItemName, process);
            }

            ProcessingManager.ProcessCreated.AddListener(AddNewProcessUI);
        });
    }

    private void Update()
    {
        float boostRatio = (ProcessingManager.instance.NextBoostTime - Time.time) / ProcessingManager.instance.BoostCooldown;
        boostButton.image.color = Color.Lerp(Color.red, Color.white, boostRatio);
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

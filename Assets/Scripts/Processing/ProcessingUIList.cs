using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcessingUIList : MonoBehaviour
{

    [SerializeField] ItemProcessingUI processUIPrefab;
    [SerializeField] Transform processList;
    [SerializeField] Toggle boostToggle;

    [SerializeField] Animator anim;


    private void Start()
    {
        boostToggle.onValueChanged.AddListener(BoostToggleChanged);

    }

    private void BoostToggleChanged(bool isOn)
    {
        bool canBoost = ProcessingManager.instance.CanBoost();
        if (!boostToggle.isOn && canBoost)
        {
            BoostProcesses();
        }

        Debug.Log("Value changed");
    }

    private void Update()
    {
        // float boostRatio = (ProcessingManager.instance.NextBoostTime - Time.time) / ProcessingManager.instance.BoostCooldown;
        // boostToggle.image.color = Color.Lerp(boostReadyColor, Color.white, boostRatio);

        if (ProcessingManager.instance.CanBoost())
        {
            boostToggle.isOn = true;
            boostToggle.interactable = true;
        }
        else
        {
            boostToggle.isOn = false;
            boostToggle.interactable = false;
        }
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
        boostToggle.SetIsOnWithoutNotify(false);
        anim.SetTrigger("boosted");
    }
}

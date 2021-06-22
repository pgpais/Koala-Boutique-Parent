using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MissionInfoScreen : MonoBehaviour
{
    [SerializeField] TMP_Text seedText;
    [SerializeField] TMP_Text zoneText;
    [SerializeField] TMP_Text difficultyText;
    [SerializeField] TMP_Text missionCompletedText;
    [SerializeField] TMP_Text diseasedItemText;
    [SerializeField] TMP_Text spawnGatherable;
    [Space]
    [SerializeField] GameObject createMissionScreen;
    [SerializeField] Button createMissionButton;

    private void OnEnable()
    {
        // Get mission from manager and update values
        Mission currentMission = GetMission();
        if (currentMission != null)
        {
            UpdateUI(currentMission);
        }
        else
        {
            ClearUI();
        }

        // listen for onChangeEvents 
        MissionManager.MissionUpdated.AddListener(OnMissionUpdated);

        createMissionButton.onClick.AddListener(SwitchToCreateMissionScreen);
    }

    private void ClearUI()
    {
        seedText.text = "";
        zoneText.text = "";
        difficultyText.text = "";
        missionCompletedText.text = "";
        spawnGatherable.text = "";
        diseasedItemText.text = "";
    }

    private Mission GetMission()
    {
        return MissionManager.instance.CurrentMission;
    }

    private void UpdateUI(Mission mission)
    {
        seedText.text = mission.seed.ToString();
        zoneText.text = mission.zone.ToString();
        difficultyText.text = mission.difficulty.ToString();
        missionCompletedText.text = mission.completed.ToString();
        diseasedItemText.text = mission.diseasedItemName;
        spawnGatherable.text = mission.gatherableItemName;
    }

    private void OnDisable()
    {
        // remove event listeners
        MissionManager.MissionUpdated.RemoveListener(OnMissionUpdated);
    }

    private void OnMissionUpdated(Mission mission)
    {
        UpdateUI(mission);
    }

    private void SwitchToCreateMissionScreen()
    {
        gameObject.SetActive(false);
        createMissionScreen.SetActive(true);
    }
}

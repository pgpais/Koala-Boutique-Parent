using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CreateNewMissionScreen : MonoBehaviour
{
    [SerializeField] TMP_Dropdown missionZone;
    [SerializeField] TMP_Dropdown missionDifficulty;
    // TODO: #11 Add items to missions

    [SerializeField] Button createMissionButton;

    [Tooltip("Used to switch back to screen")]
    [SerializeField] GameObject missionInfoScreen;
    [SerializeField] Button BackButton;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        PopulateDropdowns();

        createMissionButton.onClick.AddListener(OnCreateMission);
    }

    private void OnEnable()
    {
        BackButton.onClick.AddListener(SwitchToMissionInfoScreen);
    }

    private void OnDisable()
    {
        BackButton.onClick.RemoveListener(SwitchToMissionInfoScreen);
    }

    void OnCreateMission()
    {
        var zone = (MissionZone)missionZone.value;
        var difficulty = (MissionDifficulty)missionDifficulty.value;

        MissionManager.instance.CreateMission(zone, difficulty);
    }

    void PopulateDropdowns()
    {
        missionZone.ClearOptions();
        string[] zoneNames = Enum.GetNames(typeof(MissionZone));
        missionZone.AddOptions(new List<string>(zoneNames));

        missionDifficulty.ClearOptions();
        string[] difficultyNames = Enum.GetNames(typeof(MissionDifficulty));
        missionDifficulty.AddOptions(new List<string>(difficultyNames));
    }

    void SwitchToMissionInfoScreen()
    {
        gameObject.SetActive(false);
        missionInfoScreen.SetActive(true);
    }
}

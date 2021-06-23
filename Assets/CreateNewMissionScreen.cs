using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CreateNewMissionScreen : MonoBehaviour
{
    [SerializeField] TMP_Dropdown missionZone;
    [SerializeField] TMP_Dropdown missionDifficulty;
    [SerializeField] TMP_Dropdown missionGatherable;
    [Tooltip("Buffs that can be sent on the mission")]
    [SerializeField] BuffList supportBuffs;
    [SerializeField] ToggleGroup supportBuffsGroup;
    [SerializeField] BuffUI buffUIPrefab;
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
        PopulateSupportItems();

        createMissionButton.onClick.AddListener(OnCreateMission);
    }

    private void OnEnable()
    {
        BackButton.gameObject.SetActive(true);
        BackButton.onClick.AddListener(SwitchToMissionInfoScreen);
    }

    private void OnDisable()
    {
        BackButton.gameObject.SetActive(false);
        BackButton.onClick.RemoveListener(SwitchToMissionInfoScreen);
    }

    void OnCreateMission()
    {
        var zone = (MissionZone)missionZone.value;
        var difficulty = (MissionDifficulty)missionDifficulty.value;
        List<string> buffNames = new List<string>();
        foreach (var toggle in supportBuffsGroup.ActiveToggles())
        {
            BuffUI buff = toggle.GetComponentInParent<BuffUI>();
            buffNames.Add(buff.BuffName);
        }

        MissionManager.instance.CreateMission(zone, difficulty, buffNames, missionGatherable.captionText.text);
        SwitchToMissionInfoScreen();
    }

    void PopulateDropdowns()
    {
        missionZone.ClearOptions();
        string[] zoneNames = Enum.GetNames(typeof(MissionZone));
        missionZone.AddOptions(new List<string>(zoneNames));

        missionDifficulty.ClearOptions();
        string[] difficultyNames = Enum.GetNames(typeof(MissionDifficulty));
        missionDifficulty.AddOptions(new List<string>(difficultyNames));

        missionGatherable.ClearOptions();
        List<string> gatherableNames = ItemManager.instance.itemsData.Items.Where(item => item.Type == Item.ItemType.Gatherable).Select(item => item.ItemName).ToList();
        missionGatherable.AddOptions(gatherableNames);
    }

    void PopulateSupportItems()
    {
        foreach (var buff in supportBuffs.buffs)
        {
            Instantiate(buffUIPrefab, supportBuffsGroup.transform).Init(buff, supportBuffsGroup);
        }
    }

    void SwitchToMissionInfoScreen()
    {
        gameObject.SetActive(false);
        missionInfoScreen.SetActive(true);
        BackButton.gameObject.SetActive(false);
    }
}

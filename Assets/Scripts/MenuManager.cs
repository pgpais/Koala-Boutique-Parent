using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Button createMissionButton;

    [Tooltip("FOR DEBUG. Text for showing generated mission info")]
    [SerializeField] TMP_Text missionInfo;

    // Start is called before the first frame update
    void Start()
    {
        createMissionButton.onClick.AddListener(OnCreateMission);
        GameManager.MissionGenerated.AddListener(OnMissionGenerated);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCreateMission()
    {
        GameManager.instance.GenerateMission();
    }

    void OnMissionGenerated(Mission mission)
    {
        missionInfo.text = mission.seed.ToString();
    }
}

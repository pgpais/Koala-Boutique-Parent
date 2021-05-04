using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] GameObject menuObject;

    [SerializeField] Button createMissionButton;

    [Tooltip("FOR DEBUG. Text for showing generated mission info")]
    [SerializeField] TMP_Text missionInfo;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        menuObject.SetActive(false);


        createMissionButton.onClick.AddListener(OnCreateMission);
        GameManager.MissionGenerated.AddListener(OnMissionGenerated);
        GameManager.MissonCompleted.AddListener(OnMissionCompleted);
        FirebaseCommunicator.LoggedIn.AddListener(OnLoggedIn);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCreateMission()
    {
        GameManager.instance.GenerateMission();

        createMissionButton.gameObject.SetActive(false);
    }

    void OnMissionGenerated(Mission mission)
    {
        UpdateMissionInfoText(mission);

        createMissionButton.gameObject.SetActive(true);
    }

    void OnLoggedIn()
    {
        menuObject.SetActive(true);
    }

    void OnMissionCompleted(Mission mission)
    {
        UpdateMissionInfoText(mission);
    }

    void UpdateMissionInfoText(Mission mission)
    {
        if (mission != null)
        {
            missionInfo.text = "Seed: " + mission.seed.ToString() + "\n" + "Success: " + mission.successfulRun.ToString();
        }
        else
        {
            missionInfo.text = "Failed to get mission info";
        }
    }
}

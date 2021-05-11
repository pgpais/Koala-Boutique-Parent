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
    [SerializeField] Button addItemButton;
    [SerializeField] Button logoutButton;

    [Header("Ask for ID objects")]
    [SerializeField] GameObject askForIDParent;
    [SerializeField] Button submitFamilyIDButton;
    [SerializeField] TMP_InputField submitFamilyIDInputField;

    [Space]
    [SerializeField] GameObject ConnectingText;

    [Header("DEBUG")]

    [Tooltip("FOR DEBUG. Text for showing generated mission info")]
    [SerializeField] TMP_Text missionInfo;
    [SerializeField] string itemNameForAdding;


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
        askForIDParent.SetActive(false);
        ConnectingText.SetActive(true);


        createMissionButton.onClick.AddListener(OnCreateMission);
        addItemButton.onClick.AddListener(() => OnAddItem(itemNameForAdding));

        submitFamilyIDButton.onClick.AddListener(() => OnSubmitFamilyID(submitFamilyIDInputField.text));
        submitFamilyIDInputField.onSubmit.AddListener(OnSubmitFamilyID);

        GameManager.MissionGenerated.AddListener(OnMissionGenerated);
        GameManager.MissonCompleted.AddListener(OnMissionCompleted);
        FirebaseCommunicator.LoggedIn.AddListener(OnLoggedIn);

        logoutButton.onClick.AddListener(OnLogoutButton);

    }

    void OnLogoutButton()
    {
        PlayerPrefs.DeleteKey(PlayerSettingsKeys.familyId);

        menuObject.SetActive(false);
        askForIDParent.SetActive(true);
    }

    void OnSubmitFamilyID(string familyId)
    {
        PlayerPrefs.SetInt(PlayerSettingsKeys.familyId, int.Parse(familyId));
        FirebaseCommunicator.instance.StartGame();

        menuObject.SetActive(true);
        askForIDParent.SetActive(false);
    }

    void OnCreateMission()
    {
        GameManager.instance.GenerateMission();

        createMissionButton.gameObject.SetActive(false);
    }

    void OnAddItem(string itemName)
    {
        ItemManager.instance.AddItem(itemName, 2);
    }

    void OnMissionGenerated(Mission mission)
    {
        UpdateMissionInfoText(mission);

        createMissionButton.gameObject.SetActive(true);
    }

    void OnLoggedIn()
    {
        ConnectingText.SetActive(false);

        bool hasID = PlayerPrefs.HasKey(PlayerSettingsKeys.familyId);
        askForIDParent.SetActive(!hasID);
        menuObject.SetActive(hasID);

        if (hasID)
        {
            FirebaseCommunicator.instance.StartGame();
        }
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

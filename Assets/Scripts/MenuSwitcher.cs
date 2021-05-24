using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

public class MenuSwitcher : SerializedMonoBehaviour
{
    public static MenuSwitcher instance;

    [SerializeField] GameObject askForIdObject;
    [SerializeField] GameObject mainMenuObject;

    [SerializeField] TMP_Dropdown menuDropdown;
    [SerializeField] Button logoutButton;

    [SerializeField] string defaultScreenName = "Processing";
    [SerializeField] Dictionary<string, GameObject> menus;

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

        askForIdObject.SetActive(true);
        mainMenuObject.SetActive(false);

        FirebaseCommunicator.LoggedIn.AddListener(() =>
        {
            Debug.Log("Logged in! Enabling menu...");
            askForIdObject.SetActive(false);
            mainMenuObject.SetActive(true);


            PopulateDropdownAndSetDefault();
            // TODO: #12 Figure out another flow for starting the game
        });
        FirebaseCommunicator.LoggedOut.AddListener(OnLoggedOut);
        menuDropdown.onValueChanged.AddListener(SwitchToMenu);
        logoutButton.onClick.AddListener(Logout);
    }

    private void Start()
    {
        PopulateDropdownAndSetDefault();

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }
    }

    void PopulateDropdownAndSetDefault()
    {
        menuDropdown.ClearOptions();

        var keys = new List<string>(menus.Keys);
        menuDropdown.AddOptions(keys);

        menuDropdown.value = keys.FindIndex((key) => key == defaultScreenName);
    }

    void SwitchToMenu(int menuIndex)
    {
        var keys = new List<string>(menus.Keys);

        for (var i = 0; i < keys.Count; i++)
        {
            if (i == menuIndex)
            {
                menus[keys[i]].SetActive(true);
            }
            else
            {
                menus[keys[i]].SetActive(false);
            }
        }
    }

    void Logout()
    {
        FirebaseCommunicator.instance.Logout();
    }

    void OnLoggedOut()
    {
        askForIdObject.SetActive(true);
        mainMenuObject.SetActive(false);
    }
}

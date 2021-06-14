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

    [SerializeField] ToggleGroup tabLayoutGroup;
    [SerializeField] Toggle tabPrefab;
    [SerializeField] Sprite toggledSprite;
    [SerializeField] Sprite untoggledSprite;

    [SerializeField] string defaultScreenName = "Processing";
    [SerializeField] Dictionary<string, GameObject> menus;

    Dictionary<string, Toggle> tabs;

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

        tabs = new Dictionary<string, Toggle>();

        askForIdObject.SetActive(true);
        mainMenuObject.SetActive(false);

        FirebaseCommunicator.LoggedIn.AddListener(() =>
        {
            Debug.Log("Logged in! Enabling menu...");
            askForIdObject.SetActive(false);
            mainMenuObject.SetActive(true);


            // PopulateDropdownAndSetDefault();
            // TODO: #12 Figure out another flow for starting the game
        });
        FirebaseCommunicator.LoggedOut.AddListener(OnLoggedOut);
        // menuDropdown.onValueChanged.AddListener(SwitchToMenu);
        // logoutButton.onClick.AddListener(Logout);
    }

    private void Start()
    {
        // PopulateDropdownAndSetDefault();
        foreach (Transform child in tabLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var menu in menus)
        {
            Toggle tab = Instantiate(tabPrefab, tabLayoutGroup.transform);
            TMP_Text text = tab.GetComponentInChildren<TMP_Text>();
            // TODO: tab listener
            tab.onValueChanged.AddListener((toggled) => { HandleToggleEvent(toggled, menu.Key); });
            tab.group = tabLayoutGroup;
            text.text = menu.Key;

            tabs.Add(menu.Key, tab);


        }

        tabs[defaultScreenName].isOn = true;

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }
    }

    // void PopulateDropdownAndSetDefault()
    // {
    //     menuDropdown.ClearOptions();

    //     var keys = new List<string>(menus.Keys);
    //     menuDropdown.AddOptions(keys);

    //     menuDropdown.value = keys.FindIndex((key) => key == defaultScreenName);
    // }

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

    void SwitchToMenu(string menuKey)
    {
        foreach (var menu in menus)
        {
            if (menu.Key == menuKey)
            {
                menus[menuKey].SetActive(true);
            }
            else
            {
                menus[menu.Key].SetActive(false);
            }
        }
    }

    void HandleToggleEvent(bool toggled, string menuKey)
    {
        Image image = tabs[menuKey].GetComponentInChildren<Image>();
        if (toggled)
        {
            // change sprite to toggled
            image.sprite = toggledSprite;

            // change menu
            SwitchToMenu(menuKey);
        }
        else
        {
            // change sprite to untoggled
            image.sprite = untoggledSprite;
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

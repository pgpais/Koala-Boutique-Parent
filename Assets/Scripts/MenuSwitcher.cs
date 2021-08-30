using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuSwitcher : SerializedMonoBehaviour
{
    public static MenuSwitcher instance;

    [SerializeField] Button kingOfferingButton;
    [SerializeField] Button secretCodeButton;
    [SerializeField] Button logoutButton;

    [SerializeField] GameObject askForIdObject;
    [SerializeField] GameObject mainMenuObject;
    [SerializeField] GameObject FadeObject;
    [SerializeField] GameObject popupsParent;
    [SerializeField] GameObject KingOfferingScreen;
    [SerializeField] GameObject kingOfferingTutorialScreen;
    [SerializeField] GameObject kingOfferingTutorialArrow;
    [SerializeField] GameObject SecretDoorCodeScreen;
    [SerializeField] GameObject SecretDoorCodeTutorial;
    [SerializeField] GameObject SecretDoorCodeTutorialArrow;
    [SerializeField] GameObject DiseasedItemTutorial;
    [SerializeField] TMP_Text DiseasedItemTutorialText;
    [SerializeField] Image DiseasedItemTutorialImage;
    [SerializeField] TMP_Text DiseasedItemCoinLossText;

    [SerializeField] ToggleGroup tabLayoutGroup;
    [SerializeField] Toggle tabPrefab;
    [SerializeField] Sprite toggledSprite;
    [SerializeField] Sprite untoggledSprite;

    [SerializeField] string defaultScreenName = "Processing";
    [SerializeField] List<string> tabOrder = new List<string>();
    [SerializeField] Dictionary<string, GameObject> menus;
    [SerializeField] Dictionary<string, Sprite> tabIconImages = new Dictionary<string, Sprite>();

    Dictionary<string, Toggle> tabs;
    private Button fadeButton;

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

#if UNITY_EDITOR
        // PlayerPrefs.SetInt("SecretCodeTutorial", 0);
        // PlayerPrefs.SetInt("KingOfferingTutorial", 0);
        // PlayerPrefs.SetInt("DiseasedItemTutorial", 0);
#endif


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
        logoutButton.onClick.AddListener(Logout);
        kingOfferingButton.onClick.AddListener(ShowKingOfferingScreen);
        OfferingManager.OnOfferingChanged.AddListener(HandleKingButton);
        if (SecretDoorManager.instance != null && SecretDoorManager.instance.IsCodeDecrypted && !SecretDoorManager.instance.IsDoorUnlocked)
        {
            HandleSecretCodeButton(SecretDoorManager.instance.GetCode());
        }
        else
        {
            secretCodeButton.gameObject.SetActive(false);
            SecretDoorManager.OnCodeDecrypted.AddListener(HandleSecretCodeButton);
        }


        int DiseasedItemTutorial = PlayerPrefs.GetInt("DiseasedItemTutorial", 0);
        if (DiseasedItemTutorial == 0)
        {
            HomeScreen.OnDiseasedItemReceived.AddListener(ShowDiseasedItemTutorial);
        }

        Localisation.SetLanguage((Language)PlayerPrefs.GetInt("Language", 0));
    }

    private void ShowDiseasedItemTutorial(Item item, int diseasedItemCoinLoss)
    {
        FadeObject.SetActive(true);
        popupsParent.SetActive(true);
        DiseasedItemTutorial.SetActive(true);

        DiseasedItemTutorialImage.sprite = item.ItemSprite;
        DiseasedItemTutorialText.text = Localisation.Get(StringKey.DiseasedItemTutorial);
        DiseasedItemCoinLossText.text = diseasedItemCoinLoss.ToString();

        PlayerPrefs.SetInt("DiseasedItemTutorial", 1);
    }

    private void HandleSecretCodeButton(string code)
    {
        Debug.Log("Code decrypted! Enabling secret code button...");
        secretCodeButton.gameObject.SetActive(true);
        int secretCodeTutorial = PlayerPrefs.GetInt("SecretCodeTutorial", 0);
        if (secretCodeTutorial == 0)
        {
            ShowSecretCodeTutorial();
            PlayerPrefs.SetInt("SecretCodeTutorial", 1);
        }
        secretCodeButton.onClick.AddListener(ShowSecretCodeScreen);
    }

    void ShowSecretCodeScreen()
    {
        FadeObject.SetActive(true);
        popupsParent.SetActive(true);
        SecretDoorCodeScreen.SetActive(true);
    }

    private void ShowSecretCodeTutorial()
    {
        FadeObject.SetActive(true);
        popupsParent.SetActive(true);
        SecretDoorCodeTutorial.SetActive(true);
        SecretDoorCodeTutorialArrow.SetActive(true);
    }

    private void Start()
    {
        fadeButton = FadeObject.GetComponent<Button>();
        fadeButton.onClick.AddListener(HideAllPopups);

        // PopulateDropdownAndSetDefault();
        foreach (Transform child in tabLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var tabName in tabOrder)
        {
            Toggle tab = Instantiate(tabPrefab, tabLayoutGroup.transform);
            // TMP_Text text = tab.GetComponentInChildren<TMP_Text>();
            List<Image> images = new List<Image>(tab.GetComponentsInChildren<Image>());
            Image image = images.Find((image) => image.gameObject.name == "Icon");
            // TODO: tab listener
            tab.onValueChanged.AddListener((toggled) => { HandleToggleEvent(toggled, tabName); });
            tab.group = tabLayoutGroup;
            // text.text = menu.Key;
            if (tabIconImages.ContainsKey(tabName))
            {
                image.sprite = tabIconImages[tabName];
            }
            else
            {
                Debug.LogError("aiusdhauishda  " + tabName);
            }

            tabs.Add(tabName, tab);


        }

        tabs[defaultScreenName].isOn = true;

    }

    private void HideAllPopups()
    {
        foreach (Transform child in popupsParent.transform)
        {
            child.gameObject.SetActive(false);
        }
        popupsParent.SetActive(false);
        FadeObject.SetActive(false);
        kingOfferingTutorialArrow.SetActive(false);
        SecretDoorCodeTutorialArrow.SetActive(false);
    }

    public void ShowFadePanel()
    {
        FadeObject.SetActive(true);
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

    void ShowKingOfferingScreen()
    {
        FadeObject.SetActive(true);
        popupsParent.SetActive(true);
        KingOfferingScreen.SetActive(true);
    }

    private void ShowKingOfferingTutorialScreen()
    {
        FadeObject.SetActive(true);
        popupsParent.SetActive(true);
        kingOfferingTutorialScreen.SetActive(true);
        kingOfferingTutorialArrow.SetActive(true);
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

    void SwitchToMenu(string menuKey)
    {
        LogsManager.SendLogDirectly(new Log(
            LogType.MobileTabSwitched,
            new Dictionary<string, string>(){
                {"menu", menuKey}
            }
        ));

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
        SceneManager.LoadScene(0);
    }

    void OnLoggedOut()
    {
        askForIdObject.SetActive(true);
        mainMenuObject.SetActive(false);
    }

    void HandleKingButton()
    {
        if (OfferingManager.Instance.ShouldShowButton())
        {
            // TODO: If first time, show small tutorial popup
            int kingOfferingTutorial = PlayerPrefs.GetInt("KingOfferingTutorial", 0);
            if (kingOfferingTutorial == 0)
            {
                ShowKingOfferingTutorialScreen();
                PlayerPrefs.SetInt("KingOfferingTutorial", 1);
            }
        }

        Debug.Log("Enable button = " + OfferingManager.Instance.ShouldShowButton());
        kingOfferingButton.gameObject.SetActive(OfferingManager.Instance.ShouldShowButton());
    }
}

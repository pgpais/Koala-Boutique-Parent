using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class AskForIDMenu : MonoBehaviour
{
    [SerializeField] Toggle englishToggle;
    [SerializeField] Toggle portugueseToggle;
    [SerializeField] TMP_InputField familyIDInputField;
    [SerializeField] Button submitButton;

    private void Awake()
    {
        portugueseToggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                Localisation.SetLanguage(Language.Portuguese);
            }
        });


        englishToggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                Localisation.SetLanguage(Language.English);
            }
        });
    }

    private void Start()
    {

        submitButton.onClick.AddListener(OnSubmit);
    }

    void OnSubmit()
    {
        string familyId = familyIDInputField.text;
        string path = Path.Combine(Application.persistentDataPath, FirebaseCommunicator.familyIDSavePath);

        // Write ID to file
        FileUtils.WriteStringToFile(path, familyId);

        FirebaseCommunicator.instance.LoginAnonymously(familyId);
    }
}

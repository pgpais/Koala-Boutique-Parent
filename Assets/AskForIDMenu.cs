using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class AskForIDMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField familyIDInputField;
    [SerializeField] Button submitButton;

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

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SecretDoorUI : MonoBehaviour
{
    [SerializeField] TMP_Text first;
    [SerializeField] TMP_Text second;
    [SerializeField] TMP_Text third;
    [SerializeField] TMP_Text fourth;

    private void OnEnable()
    {
        MenuSwitcher.instance.ShowFadePanel();
        string code = SecretDoorManager.instance.GetCode();

        LogsManager.SendLogDirectly(new Log(
            LogType.SecretCodeChecked,
            new Dictionary<string, string>(){
                {"SecretCode", code}
            }
        ));

        first.text = code.Substring(0, 1);
        second.text = code.Substring(1, 1);
        third.text = code.Substring(2, 1);
        fourth.text = code.Substring(3, 1);
    }
}
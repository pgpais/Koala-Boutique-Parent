using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwitcher : MonoBehaviour
{
    public static MenuSwitcher instance;

    [SerializeField] GameObject missionScreen;
    [SerializeField] GameObject unlocksScreen;
    [SerializeField] GameObject processingScreen;

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

    private void Start()
    {
        // SwitchToProcessingScreen();
    }

    public void SwitchToMissionScreen()
    {
        missionScreen.SetActive(true);
        unlocksScreen.SetActive(false);
        processingScreen.SetActive(false);
    }
    public void SwitchToUnlocksScreen()
    {
        missionScreen.SetActive(false);
        unlocksScreen.SetActive(true);
        processingScreen.SetActive(false);
    }
    public void SwitchToProcessingScreen()
    {
        missionScreen.SetActive(false);
        unlocksScreen.SetActive(false);
        processingScreen.SetActive(true);
    }
}

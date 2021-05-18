using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionScreen : MonoBehaviour
{
    [SerializeField] GameObject missionInfoScreen;
    [SerializeField] GameObject createMissionScreen;

    private void OnEnable()
    {
        missionInfoScreen.SetActive(true);
        createMissionScreen.SetActive(false);
    }
}

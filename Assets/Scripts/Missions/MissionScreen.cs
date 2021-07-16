using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionScreen : MonoBehaviour
{
    [Header("Difficulty Screen")]
    [SerializeField] Toggle easyToggle;
    [SerializeField] Toggle mediumToggle;
    [SerializeField] Toggle hardToggle;



    private void Start()
    {
        easyToggle.onValueChanged.AddListener(OnEasyToggle);
        mediumToggle.onValueChanged.AddListener(OnMediumToggle);
        hardToggle.onValueChanged.AddListener(OnHardToggle);
    }

    private void OnHardToggle(bool isOn)
    {
        if (isOn)
        {
            MissionManager.instance.SetDifficulty(MissionDifficulty.Hard);
        }
    }
    private void OnMediumToggle(bool isOn)
    {
        if (isOn)
        {
            MissionManager.instance.SetDifficulty(MissionDifficulty.Medium);
        }
    }

    private void OnEasyToggle(bool isOn)
    {
        if (isOn)
        {
            MissionManager.instance.SetDifficulty(MissionDifficulty.Easy);
        }
    }
}

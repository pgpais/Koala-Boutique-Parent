using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionInfoScreen : MonoBehaviour
{
    [SerializeField] TMP_Text seedText;
    [SerializeField] TMP_Text zoneText;
    [SerializeField] TMP_Text difficultyText;
    [SerializeField] TMP_Text missionCompletedText;

    [Header("Debug")]
    [SerializeField] int seed;
    [SerializeField] MissionZone zone;
    [SerializeField] MissionDifficulty difficulty;
    [SerializeField] bool missionCompleted;

    private void OnEnable()
    {
        seedText.text = seed.ToString();
        zoneText.text = zone.ToString();
        difficultyText.text = difficulty.ToString();
        missionCompletedText.text = missionCompleted.ToString();
    }
}

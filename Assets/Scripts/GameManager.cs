using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static UnityEvent<Mission> MissionGenerated = new UnityEvent<Mission>();

    public Mission GeneratedMission { get; private set; }

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateMission()
    {
        GeneratedMission = new Mission();
        MissionGenerated.Invoke(GeneratedMission);
    }
}

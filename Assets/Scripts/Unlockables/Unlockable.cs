using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Unlockable", menuName = "Ye Olde Shop/Unlockable", order = 0)]
public class Unlockable : ScriptableObject
{
    public UnityEvent<Unlockable> UnlockableUpdated;

    public string UnlockableName => unlockableName;
    [SerializeField] string unlockableName;
    public List<Unlockable> Requirements => requirements;
    [SerializeField] List<Unlockable> requirements;

    // TODO: list of items required for build


    [SerializeField] bool unlocked;
    public bool Unlocked
    {
        get => runTimeUnlocked;
        set
        {
            runTimeUnlocked = value;
        }
    }
    private bool runTimeUnlocked;


    private void OnEnable()
    {
        runTimeUnlocked = unlocked;
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(unlockableName))
        {
            unlockableName = this.name;
        }
    }

    public void InitializeEvent()
    {
        UnlockableUpdated = new UnityEvent<Unlockable>();
    }

    private void OnApplicationQuit()
    {

    }
}
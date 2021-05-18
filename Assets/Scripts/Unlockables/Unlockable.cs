using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Unlockable", menuName = "Ye Olde Shop/Unlockable", order = 0)]
public class Unlockable : ScriptableObject
{
    [HideInInspector]
    public UnityEvent<Unlockable> UnlockableUpdated;

    [field: SerializeField] public string UnlockableName { get; private set; }
    [field: SerializeField] public string UnlockableDescription { get; private set; }
    [field: SerializeField] public List<Unlockable> Requirements { get; private set; }
    [field: SerializeField] public UnlockableCostDictionary Cost { get; private set; }

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
        if (string.IsNullOrEmpty(UnlockableName))
        {
            UnlockableName = this.name;
        }
    }

    public void InitializeEvent()
    {
        UnlockableUpdated = new UnityEvent<Unlockable>();
    }

    private void OnApplicationQuit()
    {

    }

    internal void Unlock()
    {


        Unlocked = true;
        UnlockableUpdated.Invoke(this);
    }
}
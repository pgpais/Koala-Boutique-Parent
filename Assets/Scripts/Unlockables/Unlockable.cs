using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Unlockable", menuName = "Ye Olde Shop/Unlockable", order = 0)]
public class Unlockable : SerializedScriptableObject
{
    [HideInInspector]
    public UnityEvent<Unlockable> UnlockableUpdated;

    [field: SerializeField] public string UnlockableName { get; private set; }
    [field: SerializeField] public string UnlockableDescription { get; private set; }
    [field: SerializeField] public List<Unlockable> Requirements { get; private set; }
    [field: SerializeField] public List<UnlockableReward> Rewards { get; private set; }
    [field: SerializeField] public Dictionary<Item, int> ItemCost { get; private set; }
    [field: SerializeField] public int GoldCost { get; private set; }
    [field: SerializeField] public int GemCost { get; private set; }

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

    public bool CanUnlock()
    {
        foreach (var unlockable in Requirements)
        {
            if (!unlockable.Unlocked)
                return false;
        }

        return true;
    }

    internal void Unlock()
    {
        Unlocked = true;

        if (Rewards != null)
        {
            foreach (UnlockableReward reward in Rewards)
            {
                reward.GetReward();
            }
        }

        UnlockableUpdated.Invoke(this);
    }
}
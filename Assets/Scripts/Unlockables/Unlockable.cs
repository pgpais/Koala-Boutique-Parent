using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Unlockable", menuName = "Ye Olde Shop/Unlockable", order = 0)]
public class Unlockable : SerializedScriptableObject, IComparable<Unlockable>
{
    [HideInInspector]
    public UnityEvent<Unlockable> UnlockableUpdated;

    [field: SerializeField] public Sprite UnlockableSprite { get; private set; }

    public string UnlockableName => Localisation.Get(UnlockableStringKey) + " " + UnlockableNameAddition;
    public StringKey UnlockableStringKey;
    public string UnlockableKeyName => Localisation.Get(UnlockableStringKey, Language.English) + " " + UnlockableNameAddition;
    public string UnlockableNameAddition;
    public string UnlockableDescription => Localisation.Get(UnlockableDescriptionStringKey);
    public StringKey UnlockableDescriptionStringKey;
    [field: SerializeField] public UnlockableType Type { get; private set; }
    [field: SerializeField] public List<Unlockable> Requirements { get; private set; }
    [field: SerializeField] public List<UnlockableReward> Rewards { get; private set; }
    [field: SerializeField] public Dictionary<Item, int> ItemCost { get; private set; }
    [field: SerializeField] public int GoldCost { get; private set; }
    [field: SerializeField] public int GemCost { get; private set; }
    [field: SerializeField] public int DifficultyModifier { get; private set; } = 0;

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
        Reset();
    }

    public void Reset()
    {
        runTimeUnlocked = unlocked;
    }

    public void InitializeEvent()
    {
        UnlockableUpdated = new UnityEvent<Unlockable>();
    }

    private void OnApplicationQuit()
    {

    }

    public bool RequirementsUnlocked()
    {
        if (Requirements == null || Requirements.Count == 0)
        {
            return true;
        }

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

    public int CompareTo(Unlockable other)
    {
        if (this.Unlocked && !other.Unlocked)
        {
            return 1;
        }
        if (!this.Unlocked && other.Unlocked)
        {
            return -1;
        }
        return this.UnlockableKeyName.CompareTo(other.UnlockableKeyName);
    }

    public bool CanUnlock()
    {
        if (GoldManager.instance.CurrentGold < GoldCost)
        {
            return false;
        }
        if (GoldManager.instance.CurrentGems < GemCost)
        {
            return false;
        }

        foreach (var itemCost in ItemCost)
        {
            bool hasEnough = ItemManager.instance.HasEnoughItem(itemCost.Key.ItemNameKey, itemCost.Value);

            if (!hasEnough)
            {
                return false;
            }
        }

        return true;
    }
}

public enum UnlockableType
{
    Items,
    Adventurer,
    Shop,
    Altar
}
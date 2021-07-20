using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "Item", menuName = "Ye Olde Shop/Item", order = 0)]
public class Item : ScriptableObject, UnlockableReward, IComparable<Item>
{
    public enum ItemType
    {
        Gatherable,
        Lootable,
        Valuable,
        Processed
    }

    public static UnityEvent<Item> ItemUnlocked { get; private set; }
    // public UnityEvent<int> ItemUpdated { get; private set; }
    // public UnityEvent ItemRemoved { get; private set; }
    [field: SerializeField] public Sprite ItemSprite { get; private set; }
    [field: SerializeField] public string ItemName { get; private set; }
    [field: SerializeField] public string Description { get; set; }
    [field: SerializeField] public ItemType Type { get; private set; }


    public Item ProcessResult => processResult;
    public int ProcessResultAmount => (int)(processResultAmount * ProcessAmountMultiplier);
    public float BoostTimeAmount => boostTimeAmount;
    public float BoostCooldown => boostCooldown;
    public float ProcessDuration => processDuration * ProcessDurationMultiplier;


    [Header("Processing")]
    [HideIf("@this.Type == ItemType.Valuable || this.Type == ItemType.Processed")]
    [SerializeField] Item processResult;
    [HideIf("@this.Type == ItemType.Valuable || this.Type == ItemType.Processed")]
    [SerializeField] int processResultAmount = 1;
    [HideIf("@this.Type == ItemType.Valuable || this.Type == ItemType.Processed")]
    [SerializeField] float boostTimeAmount = 5f;
    [HideIf("@this.Type == ItemType.Valuable || this.Type == ItemType.Processed")]
    [SerializeField] float boostCooldown = 5f;
    [HideIf("@this.Type == ItemType.Valuable || this.Type == ItemType.Processed")]
    [SerializeField] float processDuration = 15f;

    [field: SerializeField] public int GoldValue { get; private set; } = 100;
    [field: SerializeField] public int MaxModifier { get; private set; } = 10;
    [field: SerializeField] public int MinModifier { get; private set; } = -10;

    public float ProcessDurationMultiplier { get; set; } = 1f;
    public float ProcessAmountMultiplier { get; set; } = 1f;

    public bool Unlocked;

    [SerializeField] bool startsUnlocked;

    private void OnEnable()
    {
        Unlocked = startsUnlocked;
        ProcessDurationMultiplier = 1f;
        ProcessAmountMultiplier = 1f;
    }

    internal void InitializeEvent()
    {
        // ItemUpdated = new UnityEvent<int>();
        ItemUnlocked = new UnityEvent<Item>();
    }

    public void GetReward()
    {
        if (!Unlocked)
        {
            ItemUnlocked.Invoke(this);
            Unlocked = true;
        }
    }

    public int CompareTo(Item other)
    {
        if (this.processResult != null && (other.processResult == null || !other.processResult.Unlocked))
        {
            return -1;
        }
        else if ((this.processResult == null || !this.processResult.Unlocked) && other.processResult != null)
        {

            return 1;
        }
        else
        {
            return this.ItemName.CompareTo(other.ItemName);
        }
    }
}
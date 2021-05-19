using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "Item", menuName = "Ye Olde Shop/Item", order = 0)]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        Gatherable,
        Lootable,
        Valuable,
        Processed
    }


    public UnityEvent<int> ItemUpdated { get; private set; }
    public UnityEvent ItemRemoved { get; private set; }
    [field: SerializeField] public string ItemName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public ItemType Type { get; private set; }


    public Item ProcessResult => processResult;
    public float BoostTimeAmount => boostTimeAmount;
    public float BoostCooldown => boostCooldown;
    public float ProcessDuration => processDuration;


    [Header("Processing")]
    [HideIf("@this.Type == ItemType.Valuable || this.Type == ItemType.Processed")]
    [SerializeField] Item processResult;
    [HideIf("@this.Type == ItemType.Valuable || this.Type == ItemType.Processed")]
    [SerializeField] float boostTimeAmount = 5f;
    [HideIf("@this.Type == ItemType.Valuable || this.Type == ItemType.Processed")]
    [SerializeField] float boostCooldown = 5f;
    [HideIf("@this.Type == ItemType.Valuable || this.Type == ItemType.Processed")]
    [SerializeField] float processDuration = 15f;


    internal void InitializeEvent()
    {
        ItemUpdated = new UnityEvent<int>();
    }
}
using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Item", menuName = "Ye Olde Shop/Item", order = 0)]
public class Item : ScriptableObject
{
    [field: SerializeField] public UnityEvent<int> ItemUpdated { get; private set; }
    [field: SerializeField] public string ItemName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }

    internal void InitializeEvent()
    {
        ItemUpdated = new UnityEvent<int>();
    }
}
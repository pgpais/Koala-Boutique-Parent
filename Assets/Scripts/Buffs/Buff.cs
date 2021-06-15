using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Ye Olde Shop/Buff", order = 0)]
public class Buff : ScriptableObject
{
    [field: SerializeField] public Sprite icon { get; private set; }
    [field: SerializeField] public string buffName { get; private set; }
    [field: SerializeField] public string description { get; private set; }
    [field: SerializeField] public int price { get; private set; } = 100;
}

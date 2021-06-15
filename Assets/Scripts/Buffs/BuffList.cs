using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffList", menuName = "Ye Olde Shop/BuffList", order = 0)]
public class BuffList : ScriptableObject
{
    [field: SerializeField] public List<Buff> buffs { get; private set; }

    public Buff GetBuffByName(string name)
    {
        return buffs.First((buff) => string.Equals(buff.buffName, name));
    }
}

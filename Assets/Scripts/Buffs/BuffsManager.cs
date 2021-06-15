using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffsManager : MonoBehaviour
{
    [SerializeField] BuffList buffs;

    public Buff GetBuffByName(string buffName)
    {
        return buffs.GetBuffByName(buffName);
    }
}

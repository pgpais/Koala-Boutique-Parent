using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

[CreateAssetMenu(fileName = "ProcessingUpgrade", menuName = "Ye Olde Shop/ProcessingUpgrade", order = 0)]
public class ProcessingUpgrade : ScriptableObject, UnlockableReward
{
    [SerializeField] Item upgradedItem;
    [SerializeField] float processDurationMultiplier = 2f;
    [SerializeField] float processAmountMultiplier = 2f;

    public void GetReward()
    {
        upgradedItem.ProcessAmountMultiplier *= processAmountMultiplier;
        upgradedItem.ProcessDurationMultiplier *= processDurationMultiplier;
    }
}

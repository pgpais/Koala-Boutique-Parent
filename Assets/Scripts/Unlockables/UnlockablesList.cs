using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockablesList", menuName = "Ye Olde Shop/UnlockablesList", order = 0)]
public class UnlockablesList : ScriptableObject
{
    public List<Unlockable> unlockables;

    private void OnValidate()
    {
        foreach (var unlockable in unlockables)
        {
            foreach (var requirement in unlockable.Requirements)
            {
                if (!unlockables.Contains(requirement))
                {
                    Debug.LogError("Missing unlockable! Please adding it to the list... " + requirement.name);
                    unlockables.Add(requirement);
                }
            }
        }
    }
}

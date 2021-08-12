using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallTechUI : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text text;

    private Unlockable unlockable;

    public void InitUI(Unlockable unlockable)
    {
        this.unlockable = unlockable;

        text.text = unlockable.UnlockableName;
        if (unlockable.Unlocked)
            text.color = Color.green;

        unlockable.UnlockableUpdated.AddListener(UpdateUI);
    }

    void UpdateUI(Unlockable unlockable)
    {
        text.text = this.unlockable.UnlockableName;
        if (unlockable.Unlocked)
        {
            text.color = Color.green;
        }
        Debug.Log($"{unlockable.UnlockableKeyName} was updated!");
    }
}

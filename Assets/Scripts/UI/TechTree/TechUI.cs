using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechUI : MonoBehaviour
{
    [SerializeField] SmallTechUI SmallUnlockableUIPrefab;
    [Space]
    [SerializeField] Transform requirementsUI;
    [SerializeField] TMPro.TMP_Text text;
    [SerializeField] Button unlockButton;

    private Unlockable unlockable;

    private void Start()
    {
        unlockButton.onClick.AddListener(UnlockTech);
    }

    public void InitUI(Unlockable unlockable)
    {
        this.unlockable = unlockable;

        text.text = unlockable.UnlockableName;
        if (unlockable.Unlocked)
            text.color = Color.green;

        foreach (var requirement in unlockable.Requirements)
        {
            Instantiate(SmallUnlockableUIPrefab, requirementsUI).InitUI(requirement);
        }

        unlockable.UnlockableUpdated.AddListener(UpdateUI);
    }

    void UpdateUI(Unlockable unlockable)
    {
        text.text = this.unlockable.UnlockableName;
        if (unlockable.Unlocked)
        {
            text.color = Color.green;
        }
        Debug.Log($"{unlockable.UnlockableName} was updated!");
    }

    void UnlockTech()
    {
        UnlockablesManager.instance.Unlock(unlockable);
    }
}

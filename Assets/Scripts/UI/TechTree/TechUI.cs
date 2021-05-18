using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechUI : MonoBehaviour
{
    [SerializeField] SmallTechUI SmallUnlockableUIPrefab;
    [SerializeField] SmallItemUI SmallItemUIPrefab;
    [Space]
    [SerializeField] Transform requirementsUI;
    [SerializeField] Transform costUI;
    [SerializeField] TMPro.TMP_Text unlockableName;
    [SerializeField] TMPro.TMP_Text unlockableDescription;
    [SerializeField] Button unlockButton;

    private Unlockable unlockable;

    private void Start()
    {
        unlockButton.onClick.AddListener(UnlockTech);
    }

    public void InitUI(Unlockable unlockable)
    {
        this.unlockable = unlockable;

        unlockableName.text = unlockable.UnlockableName;
        unlockableDescription.text = unlockable.UnlockableDescription;
        if (unlockable.Unlocked)
            unlockableName.color = Color.green;

        InitializeRequirements(unlockable.Requirements);

        InitializeCosts(unlockable.Cost);

        unlockable.UnlockableUpdated.AddListener(UpdateUI);
    }

    void InitializeRequirements(List<Unlockable> requirements)
    {
        foreach (var requirement in requirements)
        {
            Instantiate(SmallUnlockableUIPrefab, requirementsUI).InitUI(requirement);
        }
    }

    void InitializeCosts(Dictionary<Item, int> costs)
    {
        foreach (var cost in costs)
        {
            Instantiate(SmallItemUIPrefab, costUI).InitUI(cost.Key, cost.Value);
        }
    }

    void UpdateUI(Unlockable unlockable)
    {
        unlockableName.text = this.unlockable.UnlockableName;
        if (unlockable.Unlocked)
        {
            unlockableName.color = Color.green;
        }
        Debug.Log($"{unlockable.UnlockableName} was updated!");
    }

    void UnlockTech()
    {
        UnlockablesManager.instance.Unlock(unlockable);
    }
}

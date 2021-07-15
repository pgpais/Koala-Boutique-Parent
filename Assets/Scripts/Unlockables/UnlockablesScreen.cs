using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UnlockablesScreen : SerializedMonoBehaviour
{
    [SerializeField] TechUI unlockableUIPrefab;
    [SerializeField] Transform unlockablesContainer;
    [SerializeField] Dictionary<UnlockableType, Toggle> filters = new Dictionary<UnlockableType, Toggle>();

    List<TechUI> unlockablesUI = new List<TechUI>();

    public void SetFilter()
    {
        List<UnlockableType> activeTypes = new List<UnlockableType>();

        foreach (var pair in filters)
        {
            if (pair.Value.isOn)
            {
                activeTypes.Add(pair.Key);
            }
        }

        if (activeTypes.Count == 0)
        {
            EnableAllUI();
        }
        else
        {
            foreach (var unlockableUI in unlockablesUI)
            {
                if (activeTypes.Contains(unlockableUI.Unlockable.Type))
                {
                    unlockableUI.Enable();
                }
                else
                {
                    unlockableUI.Disable();
                }
            }
        }
    }

    private void Awake()
    {
        if (UnlockablesManager.instance.GotUnlockables)
        {
            GotUnlockables();
        }
        else
        {
            UnlockablesManager.OnGotUnlockables.AddListener(GotUnlockables);
        }

        foreach (Toggle toggle in filters.Values)
        {
            toggle.onValueChanged.AddListener((isOn) => SetFilter());
        }
    }

    private void GotUnlockables()
    {
        Debug.Log("Got UNlockables");
        foreach (Unlockable unlockable in UnlockablesManager.instance.Unlockables.Values)
        {
            TechUI unlockableUI = Instantiate(unlockableUIPrefab, unlockablesContainer);
            unlockableUI.InitUI(unlockable);

            unlockablesUI.Add(unlockableUI);
        }
    }

    private void DisableAllUI()
    {
        foreach (TechUI unlockableUI in unlockablesUI)
        {
            unlockableUI.Disable();
        }
    }

    private void EnableAllUI()
    {
        foreach (TechUI unlockableUI in unlockablesUI)
        {
            unlockableUI.Enable();
        }
    }

    private void OnEnable()
    {
        //TODO: #63 Sort by costs fulfilled, then, if possible, by how close they are to be fulfilled, then by unlocked state
    }
}

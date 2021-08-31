using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class TechUI : SerializedMonoBehaviour, IComparable<TechUI>
{
    public Unlockable Unlockable => unlockable;

    [SerializeField] SmallTechUI SmallUnlockableUIPrefab;
    [SerializeField] UnlockableItemCost itemCostPrefab;
    [SerializeField] Sprite coinSprite;
    [SerializeField] Sprite gemSprite;
    [SerializeField] Dictionary<UnlockableType, Color> colorPerType;
    [Space]
    [SerializeField] Transform requirementsUI;
    [SerializeField] Transform costUI;
    [SerializeField] TMPro.TMP_Text unlockableName;
    [SerializeField] TMPro.TMP_Text unlockableDescription;
    [SerializeField] Image unlockableImage;
    [SerializeField] Button unlockButton;
    [SerializeField] GameObject unlockedObject;
    [SerializeField] GameObject divisionObject;

    private Unlockable unlockable;

    private bool requirementsUnlocked;
    private AudioSource audioSource;

    private void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
        unlockButton.onClick.AddListener(UnlockTech);
    }

    public void InitUI(Unlockable unlockable)
    {
        this.unlockable = unlockable;

        gameObject.name = unlockable.UnlockableKeyName;
        unlockableName.text = unlockable.UnlockableName;
        unlockableName.color = colorPerType[unlockable.Type];
        unlockableDescription.text = unlockable.UnlockableDescription;
        unlockableImage.sprite = unlockable.UnlockableSprite;
        // if (unlockable.Unlocked)
        //     unlockableName.color = Color.green;

        // InitializeRequirements(unlockable.Requirements);

        InitializeCosts(unlockable.ItemCost);

        unlockable.UnlockableUpdated.AddListener(UpdateUI);

        if (this.unlockable.Unlocked)
        {
            ShowUnlocked();
        }

        requirementsUnlocked = unlockable.RequirementsUnlocked();

        //todo: move this to onEnable
        if (!requirementsUnlocked)
        {
            foreach (Unlockable requirement in unlockable.Requirements)
            {
                requirement.UnlockableUpdated.AddListener(HandleUnlock);
            }
            Disable();
        }
        else
        {
            Enable();
        }

    }

    private void OnEnable()
    {
        if (!requirementsUnlocked)
        {
            Disable();
        }

        GoldManager.GoldChanged.AddListener(HandleGoldChange);
        GoldManager.GemChanged.AddListener(HandleGemChange);

        ItemManager.ItemRemoved.AddListener(HandleItemRemoved);
        ItemManager.NewItemAdded.AddListener(HandleNewItemAdded);
        ItemManager.ItemUpdated.AddListener(HandleItemUpdated);
    }

    private void OnDisable()
    {
        ItemManager.ItemRemoved.RemoveListener(HandleItemRemoved);
        ItemManager.NewItemAdded.RemoveListener(HandleNewItemAdded);
        ItemManager.ItemUpdated.RemoveListener(HandleItemUpdated);
    }

    private void HandleGemChange(int arg0)
    {
        ShowUnlockButton(unlockable.CanUnlock() && !unlockable.Unlocked);
    }

    private void HandleGoldChange(int arg0)
    {
        ShowUnlockButton(unlockable.CanUnlock() && !unlockable.Unlocked);
    }

    private void HandleItemUpdated(Item arg0, int arg1)
    {
        ShowUnlockButton(unlockable.CanUnlock() && !unlockable.Unlocked);
    }

    private void HandleNewItemAdded(Item arg0, int arg1)
    {
        ShowUnlockButton(unlockable.CanUnlock() && !unlockable.Unlocked);
    }

    private void HandleItemRemoved(Item arg0)
    {
        ShowUnlockButton(unlockable.CanUnlock() && !unlockable.Unlocked);
    }


    private void ShowUnlockButton(bool show)
    {
        if (show)
        {
            unlockButton.gameObject.SetActive(true);
        }
        else
        {
            unlockButton.gameObject.SetActive(false);
        }
    }

    void HandleUnlock(Unlockable unlockable)
    {
        if (this.unlockable.RequirementsUnlocked())
        {
            requirementsUnlocked = true;
            Enable();
        }
    }

    internal void Enable()
    {
        gameObject.SetActive(true);
        ShowUnlockButton(unlockable.CanUnlock() && !unlockable.Unlocked);
    }

    internal void Disable()
    {
        gameObject.SetActive(false);
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
        int costNumber = 0;
        GameObject rowObject = null;

        if (unlockable.GoldCost > 0)
        {
            if (costNumber % 2 == 0)
            {
                //instantiate new horizontal layout
                rowObject = CreateNewRow();
            }

            UnlockableItemCost itemCost = Instantiate(itemCostPrefab);
            itemCost.InitUI(rowObject.transform, coinSprite, "Coins", unlockable.GoldCost);

            costNumber++;
        }

        if (unlockable.GemCost > 0)
        {
            if (costNumber % 2 == 0)
            {
                //instantiate new horizontal layout
                rowObject = CreateNewRow();
            }

            UnlockableItemCost itemCost = Instantiate(itemCostPrefab);
            itemCost.InitUI(rowObject.transform, gemSprite, "Gems", unlockable.GemCost);

            costNumber++;
        }

        foreach (var cost in costs)
        {
            if (costNumber % 2 == 0)
            {
                //instantiate new horizontal layout
                rowObject = CreateNewRow();
            }
            UnlockableItemCost itemCost = Instantiate(itemCostPrefab);
            itemCost.InitUI(rowObject.transform, cost.Key, cost.Value);

            costNumber++;
        }
    }

    GameObject CreateNewRow()
    {
        GameObject rowObject = new GameObject("Cost Row");
        rowObject.transform.SetParent(costUI, false);
        HorizontalLayoutGroup layout = rowObject.AddComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandHeight = true;
        layout.childForceExpandWidth = true;
        return rowObject;
    }

    void UpdateUI(Unlockable unlockable)
    {
        unlockableName.text = this.unlockable.UnlockableName;
        if (unlockable.Unlocked)
        {
            ShowUnlocked();
        }
        Debug.Log($"{unlockable.UnlockableKeyName} was updated!");
    }

    void UnlockTech()
    {
        if (UnlockablesManager.instance.Unlock(unlockable))
        {
            if (audioSource != null)
                audioSource.Play();
        }
    }

    void ShowUnlocked()
    {
        unlockButton.gameObject.SetActive(false);
        unlockedObject.SetActive(true);

        //Hide costs
        costUI.gameObject.SetActive(false);
        divisionObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (colorPerType == null)
        {
            colorPerType = new Dictionary<UnlockableType, Color>();
            foreach (var value in Enum.GetValues(typeof(UnlockableType)))
            {
                colorPerType.Add((UnlockableType)value, Color.white);
            }
        }
    }

    public int CompareTo(TechUI other)
    {
        if (requirementsUnlocked && !other.requirementsUnlocked)
        {
            return 1;
        }
        else if (!requirementsUnlocked && other.requirementsUnlocked)
        {
            return -1;
        }
        else
        {
            return this.unlockable.CompareTo(other.unlockable);
        }
    }
}

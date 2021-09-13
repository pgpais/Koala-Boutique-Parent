using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionScreen : SerializedMonoBehaviour
{
    [SerializeField] Image nextMissionTitleImage;
    [SerializeField] Dictionary<Language, Sprite> nextMissionTitleImageSprites = new Dictionary<Language, Sprite>();

    [Header("Abundant Gatherables Screen")]
    [SerializeField] AbundantGatherableScreen abundantGatherablesScreen;
    [SerializeField] Button abundantGatherablesScreenButton;
    [SerializeField] Image abundantGatherableImage;
    [SerializeField] TMPro.TMP_Text itemNameText;
    [Space]
    [SerializeField] TMP_Text abundantItemText;
    [SerializeField] StringKey abundantItemTextStringKey;

    [Header("Diseased Gatherable Screen")]
    [SerializeField] Image diseasedImage;
    [SerializeField] TMP_Text diseasedNameText;
    [SerializeField] TMP_Text diseasedItemText;
    [SerializeField] StringKey diseasedItemTextStringKey;


    [Header("Difficulty Screen")]
    [SerializeField] Toggle easyToggle;
    [SerializeField] Toggle mediumToggle;
    [SerializeField] Toggle hardToggle;

    [Header("Daily Quest")]
    [SerializeField] Image dailyQuestTitleImage;
    [SerializeField] Dictionary<Language, Sprite> dailyQuestTitleImageSprites = new Dictionary<Language, Sprite>();
    [SerializeField] TMP_Text dailyQuestDescriptionText;
    [SerializeField] StringKey dailyQuestDescriptionTextStringKey;
    [SerializeField] SmallItemUI smallItemUIPrefab;
    [SerializeField] LayoutGroup questItemsLayout;
    [SerializeField] Image rewardImage;
    [SerializeField] TMP_Text rewardText;
    [SerializeField] StringKey rewardTextStringKey;
    [SerializeField] Image completeRewardImage;
    [SerializeField] TMP_Text completeRewardText;
    [SerializeField] StringKey completeRewardTextStringKey;
    [SerializeField] TMP_Text completeRewardDescriptionText;
    [SerializeField] StringKey completeRewardDescriptionTextStringKey;
    [SerializeField] GameObject questCompleteObject;
    [SerializeField] GameObject questRequirementsObject;


    private void Awake()
    {


        if (MissionManager.instance.GotAbundantGatherable)
        {
            SetAbundantGatherable(MissionManager.instance.AbundantGatherable);
        }
        else
        {
            MissionManager.OnGotAbundantGatherable.AddListener(SetAbundantGatherable);
        }

        if (MissionManager.instance.GotDifficulty)
        {
            SetDifficulty(MissionManager.instance.Difficulty);
        }
        else
        {
            MissionManager.OnGotDifficulty.AddListener(SetDifficulty);
        }

        if (DiseasedManager.instance.GotDiseased)
        {
            ShowDiseasedItem();
        }
        else
        {
            DiseasedManager.OnGotDiseased.AddListener(ShowDiseasedItem);
        }
    }

    private void SetDifficulty(int difficulty)
    {
        if (difficulty <= 10)
        {
            easyToggle.isOn = true;
            mediumToggle.isOn = false;
            hardToggle.isOn = false;
        }
        else if (difficulty >= 11 && difficulty <= 20)
        {
            easyToggle.isOn = false;
            mediumToggle.isOn = true;
            hardToggle.isOn = false;
        }
        else
        {
            easyToggle.isOn = false;
            mediumToggle.isOn = false;
            hardToggle.isOn = true;
        }
    }

    private void Start()
    {
        easyToggle.onValueChanged.AddListener(OnEasyToggle);
        mediumToggle.onValueChanged.AddListener(OnMediumToggle);
        hardToggle.onValueChanged.AddListener(OnHardToggle);

        abundantGatherablesScreenButton.onClick.AddListener(ShowAbundantGatherableScreen);
        abundantGatherablesScreen.OnAbundantGatherableSelected.AddListener(OnAbundantGatherableSelected);
    }

    private void OnHardToggle(bool isOn)
    {
        if (isOn)
        {
            MissionManager.instance.SetDifficulty(MissionDifficulty.Hard);
        }
    }
    private void OnMediumToggle(bool isOn)
    {
        if (isOn)
        {
            MissionManager.instance.SetDifficulty(MissionDifficulty.Medium);
        }
    }

    private void OnEasyToggle(bool isOn)
    {
        if (isOn)
        {
            MissionManager.instance.SetDifficulty(MissionDifficulty.Easy);
        }
    }

    void SetAbundantGatherable(Item item)
    {
        LogsManager.SendLogDirectly(new Log(
            LogType.AbundantItemSelected,
            new Dictionary<string, string>(){
                {"item", item.ItemNameKey}
            }
        ));

        abundantGatherableImage.sprite = item.ItemSprite;
        itemNameText.text = item.ItemName;
    }

    void ShowAbundantGatherableScreen()
    {
        abundantGatherablesScreen.Show();
    }

    void HideAbundantGatherableScreen()
    {
        abundantGatherablesScreen.Hide();
    }

    void OnAbundantGatherableSelected(Item item)
    {
        SetAbundantGatherable(item);
        MissionManager.instance.SetAbundantGatherable(item);

        HideAbundantGatherableScreen();
    }

    private void OnEnable()
    {
        nextMissionTitleImage.sprite = nextMissionTitleImageSprites[Localisation.currentLanguage];
        abundantItemText.text = Localisation.Get(abundantItemTextStringKey);
        diseasedItemText.text = Localisation.Get(diseasedItemTextStringKey);

        if (QuestManager.instance != null && QuestManager.instance.ExistsAdventurerQuest())
        {
            ShowDailyQuest();
        }
        else
        {
            HideDailyQuest();
        }
    }

    private void OnDisable()
    {
        QuestManager.AdventurerQuestCompleted.RemoveListener(ShowDailyQuestComplete);
    }

    private void HideDailyQuest()
    {
        questCompleteObject.SetActive(false);
        questRequirementsObject.SetActive(false);
    }

    private void ShowDailyQuest()
    {
        dailyQuestTitleImage.sprite = dailyQuestTitleImageSprites[Localisation.currentLanguage];

        if (QuestManager.instance.AdventurerQuestComplete())
        {
            ShowDailyQuestComplete();
        }
        else
        {
            ShowDailyQuestRequirements();
            QuestManager.AdventurerQuestCompleted.AddListener(ShowDailyQuestComplete);
        }

        QuestManager.instance.CheckAdventurerQuest();
    }

    private void ShowDailyQuestRequirements()
    {
        questCompleteObject.SetActive(false);
        questRequirementsObject.SetActive(true);

        foreach (Transform child in questItemsLayout.transform)
        {
            Destroy(child.gameObject);
        }

        Dictionary<string, int> itemQuantity = QuestManager.instance.AdventurerQuestItemQuantity;
        foreach (KeyValuePair<string, int> itemAmount in itemQuantity)
        {
            Item item = ItemManager.instance.itemsData.GetItemByName(itemAmount.Key);
            SmallItemUI itemUI = Instantiate(smallItemUIPrefab);
            itemUI.transform.SetParent(questItemsLayout.transform, false);
            itemUI.InitUI(item, itemAmount.Value);
        }

        Unlockable reward = QuestManager.instance.AdventurerQuestReward;
        rewardImage.sprite = (reward.Rewards[0] as Item).ItemSprite;

        dailyQuestDescriptionText.text = Localisation.Get(dailyQuestDescriptionTextStringKey);
        rewardText.text = Localisation.Get(rewardTextStringKey);

        QuestManager.QuestRewardChanged.AddListener(OnQuestRewardChanged);
        QuestManager.instance.UpdateAdventurerQuestReward();

    }

    private void OnQuestRewardChanged(Unlockable newReward)
    {
        rewardImage.sprite = (newReward.Rewards[0] as Item).ItemSprite;
    }

    private void ShowDailyQuestComplete()
    {
        Unlockable reward = QuestManager.instance.AdventurerQuestReward;
        questCompleteObject.SetActive(true);
        questRequirementsObject.SetActive(false);
        completeRewardImage.sprite = (reward.Rewards[0] as Item).ItemSprite;

        completeRewardText.text = Localisation.Get(completeRewardTextStringKey);
        completeRewardDescriptionText.text = Localisation.Get(completeRewardDescriptionTextStringKey).Replace("$ITEMNAME$", reward.UnlockableName);
    }

    private void ShowDiseasedItem()
    {
        Item diseasedItem = DiseasedManager.instance.DiseasedItem;

        if (diseasedItem == null)
        {
            diseasedImage.enabled = false;
            diseasedNameText.text = "";
            return;
        }

        diseasedImage.sprite = diseasedItem.ItemSprite;
        diseasedNameText.text = diseasedItem.ItemName;
    }
}

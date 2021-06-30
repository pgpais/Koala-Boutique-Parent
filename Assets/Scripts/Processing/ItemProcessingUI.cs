using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemProcessingUI : MonoBehaviour
{
    // [SerializeField] Item item;

    [Header("Parameters")]
    [SerializeField] float timeToProcess = 30f;
    [SerializeField] float timeBetweenProcessBosts = 5f;
    [SerializeField] float boostQuantity = 2f;
    [SerializeField] Color boostReadyColor = Color.red;
    Color boostBaseColor;

    [Header("UI")]
    [SerializeField] TMPro.TMP_Text itemNameText;
    [SerializeField] Image itemImage;
    Button itemButton;
    [SerializeField] Image processBoostImage;
    Button processBoostButton;
    [SerializeField] Slider processSlider;

    private Animator anim;

    private float timeElapsed;
    private float howLongForNextBoost;
    private Process process;
    private bool processing = false;

    // Start is called before the first frame update
    void Awake()
    {

    }

    public void Init(string itemName, Process process)
    {
        var item = ItemManager.instance.itemsData.GetItemByName(itemName);
        var processResult = item.ProcessResult;

        processSlider.value = 0f;

        anim = GetComponent<Animator>();

        itemButton = itemImage.GetComponent<Button>();
        processBoostButton = processBoostImage.GetComponent<Button>();

        itemImage.sprite = processResult.ItemSprite;

        timeElapsed = 0f;
        howLongForNextBoost = timeBetweenProcessBosts;

        this.process = process;
        itemNameText.text = itemName;
        boostBaseColor = processBoostImage.color;

        process.ProcessBoosted.AddListener(OnProcessBoosted);
        process.ProcessFinished.AddListener(OnProcessFinished);


        // itemButton.onClick.AddListener(StartProcessing);
        processBoostButton.onClick.AddListener(BoostProcessing);
    }

    // Update is called once per frame
    void Update()
    {
        if (process != null)
            UpdateProcessSlider();
    }

    void UpdateProcessSlider()
    {
        // processSlider.value = 1f - (process.TimeLeft / (process.DurationPerItem * process.AmountToDo));
        processSlider.value = (float)process.ElapsedTimeRatio();
        Debug.Log("Elapsed Time: " + process.ElapsedTime() + " | Ratio: " + process.ElapsedTimeRatio());
        process.HandleProcessFinish();

        AnimateBoostButton();

        if (processSlider.value >= 1f)
        {
            processing = false;
            processSlider.value = 1f;
            // ResetProcessBoost();
        }
    }

    void AnimateBoostButton()
    {
        processBoostImage.color = Color.Lerp(boostBaseColor, boostReadyColor, (float)process.ElapsedBoostTimeRatio());


        if (anim)
            anim.SetBool("BoostReady", process.CanBoost());

    }

    void ResetProcessBoost()
    {
        processBoostImage.color = boostBaseColor;
        howLongForNextBoost = timeBetweenProcessBosts;

        if (anim)
            anim.SetBool("BoostReady", false);
    }

    void StartProcessing()
    {
        if (!processing)
        {
            processing = true;
            timeElapsed = 0f;
        }
    }

    void BoostProcessing()
    {
        process.Boost();
    }

    void OnProcessBoosted()
    {
        processBoostImage.color = boostBaseColor;
    }

    void OnProcessFinished()
    {
        Destroy(gameObject);
    }
}

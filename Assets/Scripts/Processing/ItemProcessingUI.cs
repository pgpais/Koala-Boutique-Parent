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
        processSlider.value = 0f;

        anim = GetComponent<Animator>();

        itemButton = itemImage.GetComponent<Button>();
        processBoostButton = processBoostImage.GetComponent<Button>();


        timeElapsed = 0f;
        howLongForNextBoost = timeBetweenProcessBosts;
    }

    public void Init(string itemName, Process process)
    {
        this.process = process;
        itemNameText.text = itemName;
        boostBaseColor = processBoostImage.color;

        process.ProcessTick.AddListener(UpdateProcessSlider);

        // itemButton.onClick.AddListener(StartProcessing);
        // processBoostButton.onClick.AddListener(BoostProcessing);
    }

    // Update is called once per frame
    void Update()
    {
        if (processing)
        {
            // timeElapsed += Time.deltaTime;

            // AnimateProcessButton();

            // UpdateProcessSlider();
        }
    }

    void UpdateProcessSlider()
    {
        processSlider.value = 1f - (float)process.TimeLeft / (process.DurationPerItem * process.Amount);

        if (processSlider.value >= 1f)
        {
            processing = false;
            processSlider.value = 1f;
            // ResetProcessBoost();
        }
    }

    void AnimateProcessButton()
    {
        howLongForNextBoost -= Time.deltaTime;
        float boostRatio = howLongForNextBoost / timeBetweenProcessBosts;
        processBoostImage.color = Color.Lerp(boostReadyColor, boostBaseColor, boostRatio);

        anim.SetBool("BoostReady", howLongForNextBoost <= 0);

    }

    void ResetProcessBoost()
    {
        processBoostImage.color = boostBaseColor;
        howLongForNextBoost = timeBetweenProcessBosts;

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
        if (howLongForNextBoost <= 0f)
        {
            timeElapsed += boostQuantity;
            UpdateProcessSlider();
            ResetProcessBoost();
        }
    }
}

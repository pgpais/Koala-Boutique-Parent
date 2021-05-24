using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewProcessMenu : MonoBehaviour
{
    public static NewProcessMenu instance;

    [SerializeField] TMPro.TMP_Text maxAmountText;
    [SerializeField] TMPro.TMP_Text minAmountText;
    [SerializeField] Slider amountSlider;
    [SerializeField] Button startProcessButton;
    [SerializeField] Button backButton;
    [SerializeField] TMPro.TMP_Text resultText;
    private string itemName;
    private Item item;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        amountSlider.onValueChanged.AddListener(OnSliderUpdated);
        startProcessButton.onClick.AddListener(OnStartProcess);
        backButton.onClick.AddListener(CloseMenu);
    }

    public void Init(string itemName, int minAmount, int maxAmount)
    {
        item = ItemManager.instance.itemsData.GetItemByName(itemName);

        this.itemName = itemName;
        this.amountSlider.minValue = minAmount;
        minAmountText.text = minAmount.ToString();
        this.amountSlider.maxValue = maxAmount;
        maxAmountText.text = maxAmount.ToString();

        UpdateResultText(maxAmount / 2);
        amountSlider.value = maxAmount / 2;

        gameObject.SetActive(true);
        transform.parent.gameObject.SetActive(true);
    }

    void OnSliderUpdated(float value)
    {
        int currentAmount = (int)value;
        UpdateResultText(currentAmount);
        if (currentAmount == 0)
        {
            startProcessButton.interactable = false;
        }
        else
        {
            startProcessButton.interactable = true;

        }
    }

    void UpdateResultText(int currentAmount)
    {
        resultText.text = $"You will process {currentAmount} of {item.ProcessResult.ItemName} after {item.ProcessDuration * currentAmount} seconds";
    }

    void OnStartProcess()
    {
        ProcessingManager.instance.StartProcessing(itemName, (int)amountSlider.value);
        CloseMenu();
    }

    void CloseMenu()
    {
        gameObject.SetActive(false);
        transform.parent.gameObject.SetActive(false);
    }
}

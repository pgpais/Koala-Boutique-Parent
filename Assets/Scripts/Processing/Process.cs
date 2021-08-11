using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class Process
{
    private const float MAX_PROCESS_DURATION = 1800;

    [JsonIgnore]
    public UnityEvent ProcessBoosted;
    [JsonIgnore]
    public UnityEvent ProcessFinished;
    static readonly string dateStringFormat = "yyyyMMdd HH:mm:ss.ffffff";

    public int ProcessAmount => processAmount;
    public string StartDateString => startDateString;
    public string NextBoostDateString => nextBoostDateString;
    public float ProcessDuration => processDuration;
    public string ProcessingItemName => processingItemName;
    public int NumberOfBoosts => numberOfBoosts;

    [JsonIgnore]
    public string Key;

    readonly int processAmount;
    readonly string startDateString;
    string nextBoostDateString;
    readonly float processDuration;
    readonly float boostTimeAmount;
    readonly float boostCooldown;
    readonly string processingItemName;
    int numberOfBoosts;

    public Process(Item item, int amount)
    {
        processAmount = amount;
        startDateString = DateTime.Now.ToString(dateStringFormat);
        nextBoostDateString = startDateString;
        processDuration = item.ProcessDuration * amount;
        processDuration = Mathf.Clamp(processDuration, 0, MAX_PROCESS_DURATION);
        boostTimeAmount = item.BoostTimeAmount;
        boostCooldown = item.BoostCooldown;
        processingItemName = item.ItemName;
        numberOfBoosts = 0;

        ProcessFinished = new UnityEvent();
        ProcessBoosted = new UnityEvent();
    }

    [JsonConstructor]
    public Process(string processingItemName, string startDateString, int numberOfBoosts, string nextBoostDateString, int processAmount, float processDuration)
    {
        var processingItem = ItemManager.instance.itemsData.GetItemByName(processingItemName);

        this.processAmount = processAmount;
        this.startDateString = startDateString;
        this.nextBoostDateString = nextBoostDateString;
        this.processDuration = processDuration;
        this.processDuration = Mathf.Clamp(processDuration, 0, MAX_PROCESS_DURATION);
        this.boostTimeAmount = processingItem.BoostTimeAmount;
        this.boostCooldown = processingItem.BoostCooldown;
        this.processingItemName = processingItemName;
        this.numberOfBoosts = numberOfBoosts;

        ProcessFinished = new UnityEvent();
        ProcessBoosted = new UnityEvent();

        HandleProcessFinish();
    }

    public void Boost()
    {
        // if (CanBoost())
        // {
        numberOfBoosts++;
        var nextBoostDate = DateTime.Now + new TimeSpan(0, 0, (int)boostCooldown);

        nextBoostDateString = nextBoostDate.ToString(dateStringFormat);

        ProcessBoosted.Invoke();
        HandleProcessFinish();
        // }
    }

    public double ElapsedTime()
    {
        DateTime startDate = DateTime.ParseExact(startDateString, dateStringFormat, null);
        TimeSpan span = DateTime.Now - startDate;
        return span.TotalSeconds + (numberOfBoosts * boostTimeAmount);
    }

    public double ElapsedTimeRatio() => ElapsedTime() / processDuration;

    public double TimeForNextBoost()
    {
        DateTime nextBoostDate = DateTime.ParseExact(nextBoostDateString, dateStringFormat, null);
        TimeSpan span = nextBoostDate - DateTime.Now;

        return span.TotalSeconds;
    }

    public double ElapsedBoostTimeRatio()
    {
        double value = 1 - (TimeForNextBoost() / boostCooldown);

        // for some reason Math doesn't have clamp and mathf works only with floats...
        if (value > 1)
        {
            value = 1;
        }
        else if (value < 0)
        {
            value = 0;
        }

        return value;
    }


    public bool IsProcessFinished() => ElapsedTime() >= processDuration;

    public bool CanBoost() => TimeForNextBoost() <= 0;

    public void HandleProcessFinish()
    {
        if (IsProcessFinished())
        {
            ProcessFinished.Invoke();
        }
    }
}

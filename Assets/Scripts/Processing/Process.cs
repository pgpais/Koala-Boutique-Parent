using System;
using Newtonsoft.Json;
using UnityEngine.Events;

[JsonObject(MemberSerialization.OptIn)]
public class Process
{
    static readonly DateTime centuryBegin = new DateTime(2001, 1, 1);

    public UnityEvent ProcessTick;
    public UnityEvent ProcessBoosted;
    public UnityEvent ProcessItemFinished;
    public UnityEvent ProcessFinished;

    public string Key;

    [JsonProperty]
    public string ProcessedItemName { get; private set; }
    [JsonProperty]
    public string ResultItemName { get; private set; }
    [JsonProperty]
    public int ResultItemAmount { get; private set; }

    [JsonProperty]
    public int AmountToDo { get; private set; }
    public int AmountDone { get; private set; }

    [JsonProperty]
    public float DurationPerItem { get; private set; }

    [JsonProperty]
    public float TimeLeft { get; private set; }
    [JsonProperty]
    public DateTime LastTickTime { get; private set; }

    public DateTime LastBoostTime { get; private set; }
    [JsonProperty]
    public DateTime NextBoostTime { get; private set; }

    [JsonProperty]
    public float BoostCooldown { get; private set; }
    public float BoostTimeAmount { get; private set; }

    public Process(float durationPerItem, int amount, float boostTime, float boostCooldown, string resultItemName, int resultItemAmount, string processedItemName, string key)
    {
        this.Key = key;

        this.ProcessedItemName = processedItemName;
        this.ResultItemName = resultItemName;
        this.ResultItemAmount = resultItemAmount;

        this.AmountToDo = amount;
        this.AmountDone = 0;
        this.DurationPerItem = durationPerItem;

        this.TimeLeft = durationPerItem * amount;

        this.BoostTimeAmount = boostTime;
        this.BoostCooldown = boostCooldown;

        DateTime currentDate = DateTime.UtcNow;
        LastTickTime = currentDate;


        ProcessTick = new UnityEvent();
        ProcessBoosted = new UnityEvent();
        ProcessItemFinished = new UnityEvent();
        ProcessFinished = new UnityEvent();


        this.NextBoostTime = LastTickTime.AddSeconds(BoostCooldown);
    }

    [JsonConstructor]
    public Process(float durationPerItem, int amount, float boostTimeAmount, float boostCooldown, string resultItemName, int resultItemAmount, string processedItemName, string key, float timeLeft, DateTime lastTickTime, DateTime nextBoostTime)
    {
        this.Key = key;

        this.ProcessedItemName = processedItemName;
        this.ResultItemName = resultItemName;
        this.ResultItemAmount = resultItemAmount;

        this.AmountToDo = amount;
        this.AmountDone = 0;
        this.DurationPerItem = durationPerItem;

        this.TimeLeft = timeLeft;

        this.BoostTimeAmount = boostTimeAmount;
        this.BoostCooldown = boostCooldown;

        LastTickTime = lastTickTime;

        ProcessTick = new UnityEvent();
        ProcessBoosted = new UnityEvent();
        ProcessItemFinished = new UnityEvent();
        ProcessFinished = new UnityEvent();

        DoTick();

        this.NextBoostTime = nextBoostTime;

    }

    public void DoTick()
    {


        TimeSpan timeSinceLastTick = DateTime.UtcNow - LastTickTime;
        TimeLeft -= (float)timeSinceLastTick.TotalSeconds;

        LastTickTime = DateTime.UtcNow;

        HandleFinishingProcess();

        ProcessTick.Invoke();
    }

    public void Boost()
    {
        DateTime today = DateTime.UtcNow;
        if (today >= NextBoostTime)
        {
            TimeLeft -= BoostTimeAmount;

            HandleFinishingProcess();

            NextBoostTime = today.AddSeconds(BoostCooldown);

            ProcessBoosted.Invoke();
        }
    }

    void HandleFinishingProcess()
    {
        double amountToDoWithNewTime = TimeLeft / DurationPerItem;
        if ((amountToDoWithNewTime + 1) < AmountToDo)
        {
            FinishItem();
            AmountToDo--;
            // AmountDone = amountToDoWithNewTime;

            if (AmountToDo <= 0)
            {
                FinishProcess();
            }
        }
    }

    void FinishItem() => ProcessItemFinished.Invoke();

    void FinishProcess() => ProcessFinished.Invoke();

    static double TimeSinceCenturyBegin(DateTime date)
    {
        return new TimeSpan(date.Ticks - centuryBegin.Ticks).TotalSeconds;
    }
}

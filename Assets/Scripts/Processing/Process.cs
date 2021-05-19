using System;
using UnityEngine.Events;

public class Process
{
    static readonly DateTime centuryBegin = new DateTime(2001, 1, 1);

    public UnityEvent ProcessTick;
    public UnityEvent ProcessBoosted;
    public UnityEvent ProcessItemFinished;
    public UnityEvent ProcessFinished;

    public int AmountToDo { get; private set; }
    public int AmountDone { get; private set; }

    public float DurationPerItem { get; private set; }

    public double TimeLeft { get; private set; }
    public double LastTickTime { get; private set; }

    public double LastBoostTime { get; private set; }
    public double NextBoostTime { get; private set; }
    public double BoostCooldown { get; private set; }
    public float BoostTimeAmount { get; private set; }


    public Process(float durationPerItem, int amount, float boostTime, float boostCooldown)
    {
        this.AmountToDo = amount;
        this.AmountDone = 0;
        this.DurationPerItem = durationPerItem;

        this.TimeLeft = durationPerItem * amount;

        this.BoostTimeAmount = boostTime;
        this.BoostCooldown = boostCooldown;

        DateTime currentDate = DateTime.UtcNow;
        LastTickTime = TimeSinceCenturyBegin(currentDate);

        this.NextBoostTime = LastTickTime + BoostCooldown;

        ProcessTick = new UnityEvent();
        ProcessBoosted = new UnityEvent();
        ProcessItemFinished = new UnityEvent();
        ProcessFinished = new UnityEvent();
    }

    public Process(float lastTickTime, float durationPerItem, int amount, float timeLeft)
    {

    }

    public void DoTick()
    {
        double curTime = TimeSinceCenturyBegin(DateTime.UtcNow);

        double timeSinceLastTick = curTime - LastTickTime;
        TimeLeft -= timeSinceLastTick;

        HandleFinishingProcess();

        LastTickTime = curTime;

        ProcessTick.Invoke();
    }

    public void Boost()
    {
        double curTime = TimeSinceCenturyBegin(DateTime.UtcNow);

        if (curTime >= NextBoostTime)
        {
            TimeLeft -= BoostTimeAmount;

            HandleFinishingProcess();

            NextBoostTime = curTime + BoostCooldown;

            ProcessBoosted.Invoke();
        }
    }

    void HandleFinishingProcess()
    {
        int amountDoneWithNewtime = (int)(TimeLeft / DurationPerItem);
        if (amountDoneWithNewtime > AmountDone)
        {
            FinishItem();
            AmountDone = amountDoneWithNewtime;
            if (AmountDone >= AmountToDo)
            {
                FinishProcess();
            }
        }
    }

    void FinishItem()
    {

        ProcessItemFinished.Invoke();
    }

    void FinishProcess()
    {

        ProcessFinished.Invoke();
    }

    static double TimeSinceCenturyBegin(DateTime date)
    {
        return new TimeSpan(date.Ticks - centuryBegin.Ticks).TotalSeconds;
    }
}
using System;
using UnityEngine.Events;

public class Process
{
    static readonly DateTime centuryBegin = new DateTime(2001, 1, 1);

    public UnityEvent ProcessTick;
    public UnityEvent ProcessItemFinished;
    public UnityEvent ProcessFinished;

    public int Amount { get; private set; }
    public float DurationPerItem { get; private set; }
    public double TimeLeft { get; private set; }
    public double LastTickTime { get; private set; }



    public Process(float durationPerItem, int amount)
    {
        this.Amount = amount;
        this.DurationPerItem = durationPerItem;

        this.TimeLeft = durationPerItem * amount;



        DateTime currentDate = DateTime.UtcNow;
        LastTickTime = TimeSinceCenturyBegin(currentDate);

        ProcessTick = new UnityEvent();
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

        if (TimeLeft <= 0f)
        {
            FinishItem();

            if (Amount <= 0)
            {
                FinishProcess();
            }
        }

        LastTickTime = curTime;

        ProcessTick.Invoke();
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

using System;
using System.Collections.Generic;

[Serializable]
public class Mission
{
    public static string firebaseReferenceName = "missions";

    public int seed;
    public bool completed;
    public MissionZone zone;
    public MissionDifficulty difficulty;

    public List<string> BoughtBuffs;



    public Mission()
    {
        seed = new Random().Next();
        completed = false;
    }

    public Mission(MissionZone zone, MissionDifficulty difficulty, List<string> buffNames) : this()
    {
        this.zone = zone;
        this.difficulty = difficulty;
        this.BoughtBuffs = buffNames;
    }

    public string GetFirebaseReferenceName() => firebaseReferenceName;
}
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
    public string diseasedItemName;
    public string gatherableItemName;

    public List<string> boughtBuffs;



    public Mission()
    {
        seed = new Random().Next();
        completed = false;
    }

    public Mission(MissionZone zone, MissionDifficulty difficulty, List<string> buffNames) : this()
    {
        this.zone = zone;
        this.difficulty = difficulty;
        this.boughtBuffs = buffNames;
    }

    public string GetFirebaseReferenceName() => firebaseReferenceName;
}
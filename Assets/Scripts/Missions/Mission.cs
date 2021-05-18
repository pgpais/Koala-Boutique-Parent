using System;

[Serializable]
public class Mission
{
    public static string firebaseReferenceName = "missions";

    public int seed;
    public bool completed;
    public MissionZone zone;
    public MissionDifficulty difficulty;



    public Mission()
    {
        seed = new Random().Next();
        completed = false;
    }

    public Mission(MissionZone zone, MissionDifficulty difficulty)
    {
        this.zone = zone;
        this.difficulty = difficulty;
    }

    public string GetFirebaseReferenceName() => firebaseReferenceName;
}
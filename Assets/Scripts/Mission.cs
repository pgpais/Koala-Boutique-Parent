using System;

public class Mission
{
    public int seed { get; private set; }

    public Mission()
    {
        seed = new Random().Next();
    }
}
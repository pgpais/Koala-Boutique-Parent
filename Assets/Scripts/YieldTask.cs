using System.Threading.Tasks;
using UnityEngine;

public class YieldTask : CustomYieldInstruction
{
    public YieldTask(Task task)
    {
        Task = task;
    }

    public override bool keepWaiting => !Task.IsCompleted;

    public Task Task { get; }
}

public class YieldTask<A>:CustomYieldInstruction
{
    public YieldTask(Task<A> task)
    {
        Task = task;
    }

    public override bool keepWaiting => !Task.IsCompleted;

    public Task<A> Task { get; }
}
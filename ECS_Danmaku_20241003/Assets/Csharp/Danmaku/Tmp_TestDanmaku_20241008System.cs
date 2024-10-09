using Unity.Entities;
using UnityEngine;
using static DanmakuJobs;

public partial struct DanmakuSystem : ISystem
{
    private SystemHandle _ecbSystemHandle;

    public void OnCreate(ref SystemState state)
    {
        // SystemHandle‚ğæ“¾
        _ecbSystemHandle = state.World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        // ecb‚Ì€”õ
        var ecbSystem = state.World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        // Job‚ÅÀ‘•‚µ‚Ä’e–‹ˆ—‚ğŒÄ‚Ño‚·
        var jobHandle = new N_WayJob
        {
            commandBuffer = ecb,
            deltaTime = delta
        }.ScheduleParallel(state.Dependency);

        // ecb‚ğI‚¦‚é
        ecbSystem.AddJobHandleForProducer(jobHandle);
        state.Dependency = jobHandle;
    }
}

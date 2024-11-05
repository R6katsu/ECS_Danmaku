using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// ’e‚Ìˆ—
/// </summary>
[BurstCompile]
public partial struct BulletSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;
        var delta = SystemAPI.Time.DeltaTime;

        foreach (var (bullet, destroyable) in
                 SystemAPI.Query<RefRW<BulletData>,
                                 RefRW<DestroyableData>>())
        {
            bullet.ValueRW.elapsed += delta;

            // Œo‰ßŠÔ‚ªõ–½–¢–‚¾‚Á‚½
            if (bullet.ValueRW.elapsed < bullet.ValueRW.lifeTime) { continue; }

            // õ–½‚ğŒ}‚¦‚½‚Ì‚Åíœƒtƒ‰ƒO‚ğ—§‚Ä‚é
            destroyable.ValueRW.isKilled = true;
        }
    }
}

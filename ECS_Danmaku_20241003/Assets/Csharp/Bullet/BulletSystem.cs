using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// �e�̏���
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

            // �o�ߎ��Ԃ���������������
            if (bullet.ValueRW.elapsed < bullet.ValueRW.lifeTime) { continue; }

            // �������}�����̂ō폜�t���O�𗧂Ă�
            destroyable.ValueRW.isKilled = true;
        }
    }
}

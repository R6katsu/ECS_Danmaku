using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static BulletHelper;
using static EnemyHelper;

#if UNITY_EDITOR
#endif

/// <summary>
/// �G�̏���
/// </summary>
[BurstCompile]
public partial struct EnemySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // MovementRangeSingletonData�����݂��Ȃ�����
        if (!SystemAPI.HasSingleton<MovementRangeSingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var movementRangeSingleton = SystemAPI.GetSingleton<MovementRangeSingletonData>();

        // �G�̈ړ��\�͈͂��擾
        var enemyMovementRange = movementRangeSingleton.enemyMovementRange;
        var movementRange = enemyMovementRange.movementRange + enemyMovementRange.movementRangeCenter;

        // �����̑傫�������߂�
        var halfMovementRange = movementRange / 2;

        // �ړ��\�͈͊O��������폜�t���O�𗧂Ă�
        foreach (var (enemy, destroyable, localTfm) in
                 SystemAPI.Query
                 <RefRW<EnemyTag>,
                 RefRW<DestroyableData>,
                 RefRW<LocalTransform>>())
        {
            var position = localTfm.ValueRO.Position;

            // �ړ��\�͈͊O������
            if (position.x < -halfMovementRange.x || position.x > halfMovementRange.x ||
                position.y < -halfMovementRange.y || position.y > halfMovementRange.y ||
                position.z < -halfMovementRange.z || position.z > halfMovementRange.z)
            {
                // �폜�t���O�𗧂Ă�
                destroyable.ValueRW.isKilled = true;
            }
        }
    }
}

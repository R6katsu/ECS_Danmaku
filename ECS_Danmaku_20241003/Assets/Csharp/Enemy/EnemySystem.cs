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

        // ���S�ʒu
        var movementRangeCenter = enemyMovementRange.movementRangeCenter;

        // �����̑傫�������߂�
        var halfMovementRange = enemyMovementRange.movementRange / 2;

        // ���S�ʒu���l�������ړ��\�͈͂����߂�
        var minMovementRange = movementRangeCenter + -halfMovementRange;
        var maxMovementRange = movementRangeCenter + halfMovementRange;

        // �ړ��\�͈͊O��������폜�t���O�𗧂Ă�
        foreach (var (enemy, destroyable, localTfm) in
                 SystemAPI.Query
                 <RefRW<EnemyTag>,
                 RefRW<DestroyableData>,
                 RefRW<LocalTransform>>())
        {
            var position = localTfm.ValueRO.Position;

            // �ړ��\�͈͊O������
            if (position.x < minMovementRange.x || position.x > maxMovementRange.x ||
                position.y < minMovementRange.y || position.y > maxMovementRange.y ||
                position.z < minMovementRange.z || position.z > maxMovementRange.z)
            {
                // �폜�t���O�𗧂Ă�
                destroyable.ValueRW.isKilled = true;
            }
        }
    }
}

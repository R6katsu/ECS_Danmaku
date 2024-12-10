using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using static BulletHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �e�̏���
/// </summary>
[BurstCompile]
public partial struct BulletSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // MovementRangeSingletonData�����݂��Ȃ�����
        if (!SystemAPI.HasSingleton<MovementRangeSingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var movementRangeSingleton = SystemAPI.GetSingleton<MovementRangeSingletonData>();

        // �e�̈ړ��\�͈͂��擾
        var bulletMovementRange = movementRangeSingleton.BulletMovementRange;

        // ���S�ʒu
        var movementRangeCenter = bulletMovementRange.movementRangeCenter;

        // �����̑傫�������߂�
        var halfMovementRange = bulletMovementRange.movementRange / 2;

        // ���S�ʒu���l�������ړ��\�͈͂����߂�
        var minMovementRange = movementRangeCenter + -halfMovementRange;
        var maxMovementRange = movementRangeCenter + halfMovementRange;

        // �ړ��\�͈͊O��������폜�t���O�𗧂Ă�
        foreach (var (bullet, destroyable, localTfm) in
                 SystemAPI.Query
                 <RefRW<BulletTag>,
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

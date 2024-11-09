using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static MovementRangeAuthoring;

#if UNITY_EDITOR
using static HealthPointDatas;
using static PlayerHelper;
#endif

/// <summary>
/// �ړ��\�͈͂̏��i�V���O���g���O��j
/// </summary>
[Serializable]
public struct MovementRangeSingletonData : IComponentData
{
    [Header("PL�̈ړ��\�͈�")]
    public MovementRangeInfo playerMovementRange;

    [Header("�e�̈ړ��\�͈�")]
    public MovementRangeInfo bulletMovementRange;

    [Header("�G�̈ړ��\�͈�")]
    public MovementRangeInfo enemyMovementRange;
}

/// <summary>
/// �ړ��\�͈͂̐ݒ�
/// </summary>
public class MovementRangeAuthoring : SingletonMonoBehaviour<MovementRangeAuthoring>
{
    /// <summary>
    /// �ړ��\�͈͂ƒ��S�ʒu�̏��
    /// </summary>
    [Serializable]
    public struct MovementRangeInfo
    {
        [SerializeField, Header("�ړ��\�͈�")]
        public float3 movementRange;

        [SerializeField, Header("�ړ��\�͈͂̒��S�ʒu")]
        public float3 movementRangeCenter;
    }

    public MovementRangeSingletonData _playerMovementRangeSingletonData = new();

    public class Baker : Baker<MovementRangeAuthoring>
    {
        public override void Bake(MovementRangeAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, src._playerMovementRangeSingletonData);

            // �r���h���ɌŒ�
            AddComponent<StaticOptimizeEntity>(entity);
        }
    }
}

using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
using static MovementRangeAuthoring;
using System.Collections;
using System.Collections.Generic;
using static HealthPointDatas;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �ړ��\�͈͂ƒ��S�ʒu�̏��
/// </summary>
[Serializable]
public struct MovementRangeInfo
{
    [Header("�ړ��\�͈�")]
    public float3 movementRange;

    [Header("�ړ��\�͈͂̒��S�ʒu")]
    public float3 movementRangeCenter;
}

/// <summary>
/// �ړ��\�͈͂̏��i�V���O���g���O��j
/// </summary>
[Serializable]
public struct MovementRangeSingletonData : IComponentData
{
    [SerializeField, Header("PL�̈ړ��\�͈�")]
    private MovementRangeInfo _playerMovementRange;

    [SerializeField, Header("�e�̈ړ��\�͈�")]
    private MovementRangeInfo _bulletMovementRange;

    [SerializeField, Header("�G�̈ړ��\�͈�")]
    private MovementRangeInfo _enemyMovementRange;

    /// <summary>
    /// PL�̈ړ��\�͈�
    /// </summary>
    public MovementRangeInfo PlayerMovementRange => _playerMovementRange;

    /// <summary>
    /// �e�̈ړ��\�͈�
    /// </summary>
    public MovementRangeInfo BulletMovementRange => _bulletMovementRange;

    /// <summary>
    /// �G�̈ړ��\�͈�
    /// </summary>
    public MovementRangeInfo EnemyMovementRange => _enemyMovementRange;

    /// <summary>
    /// �ړ��\�͈͂̏��i�V���O���g���O��j
    /// </summary>
    /// <param name="playerMovementRange">PL�̈ړ��\�͈�</param>
    /// <param name="bulletMovementRange">�e�̈ړ��\�͈�</param>
    /// <param name="enemyMovementRange">�G�̈ړ��\�͈�</param>
    public MovementRangeSingletonData
    (
        MovementRangeInfo playerMovementRange, 
        MovementRangeInfo bulletMovementRange, 
        MovementRangeInfo enemyMovementRange
    )
    {
        _playerMovementRange = playerMovementRange;
        _bulletMovementRange = bulletMovementRange;
        _enemyMovementRange = enemyMovementRange;
    }
}

/// <summary>
/// �ړ��\�͈͂̐ݒ�
/// </summary>
public class MovementRangeAuthoring : SingletonMonoBehaviour<MovementRangeAuthoring>
{
    [SerializeField, Header("�ړ��\�͈͂̏��")]
    private MovementRangeSingletonData _playerMovementRangeSingletonData = new();

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

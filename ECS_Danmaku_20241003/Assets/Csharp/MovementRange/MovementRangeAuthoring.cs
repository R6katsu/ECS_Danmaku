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

    /*
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
        this.playerMovementRange = playerMovementRange;
        this.bulletMovementRange = bulletMovementRange;
        this.enemyMovementRange = enemyMovementRange;
    }
    */
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

    /*
    [SerializeField, Header("PL�̈ړ��\�͈�")]
    private MovementRangeInfo _playerMovementRange = new();

    [SerializeField, Header("�e�̈ړ��\�͈�")]
    private MovementRangeInfo _bulletMovementRange = new();

    [SerializeField, Header("�G�̈ړ��\�͈�")]
    private MovementRangeInfo _enemyMovementRange = new();
    */

    public class Baker : Baker<MovementRangeAuthoring>
    {
        public override void Bake(MovementRangeAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            /*
            // MovementRangeSingletonData�̃C���X�^���X���쐬
            var movementRangeSingleton = new MovementRangeSingletonData
                (
                    src._playerMovementRange,
                    src._bulletMovementRange,
                    src._enemyMovementRange
                );
            */

            //AddComponent(entity, movementRangeSingleton);
            AddComponent(entity, src._playerMovementRangeSingletonData);

            // �r���h���ɌŒ�
            AddComponent<StaticOptimizeEntity>(entity);
        }
    }
}

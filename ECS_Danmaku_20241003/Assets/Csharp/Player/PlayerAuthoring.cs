using Unity.Entities;
using UnityEngine;
using static PlayerHelper;

#if UNITY_EDITOR
using System;
using static EntityCategoryHelper;
using static HealthPointDatas;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Transforms;
using static TriggerHelper;
using static EntityCampsHelper;
using UnityEngine.Events;
using static DanmakuJobs;
using static HealthHelper;
using static PlayerAuthoring;
using static UnityEngine.EventSystems.EventTrigger;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// PL�̏��i�V���O���g���O��j
/// </summary>
public struct PlayerSingletonData : IComponentData
{
    [Tooltip("PL�ړ����̌X���̑傫��")]
    public readonly int playerMoveTilt;

    [Tooltip("�ړ����x")]
    public readonly float moveSpeed;

    [Tooltip("�������̈ړ����x")]
    public readonly float slowMoveSpeed;

    [Tooltip("�ˌ��Ԋu")]
    public readonly float firingInterval;

    [Tooltip("�`���[�W����")]
    public readonly float chargeTime;

    [Tooltip("���S���̌��ʉ��ԍ�")]
    public readonly int killedSENumber;

    [Tooltip("PL�̒e��PrefabEntity")]
    public readonly Entity playerBulletEntity;

    [Tooltip("PL�̌����ڂ�Entity")]
    public readonly Entity playerModelEntity;

    /// <summary>
    /// PL�̏��
    /// </summary>
    /// <param name="playerMoveTilt">�ړ����x</param>
    /// <param name="moveSpeed">�ړ����x</param>
    /// <param name="moveSlowSpeed">�������̈ړ����x</param>
    /// <param name="firingInterval">�ˌ��Ԋu</param>
    /// <param name="chargeTime">�`���[�W����</param>
    /// <param name="killedSENumber">���S���̌��ʉ��ԍ�</param>
    /// <param name="playerBulletEntity">PL�̒e��PrefabEntity</param>
    public PlayerSingletonData
        (
        int playerMoveTilt,
        float moveSpeed,
        float moveSlowSpeed, 
        float firingInterval,
        float chargeTime,
        int killedSENumber,
        Entity playerBulletEntity,
        Entity playerModelEntity
        )
    {
        this.playerMoveTilt = playerMoveTilt;
        this.moveSpeed = moveSpeed;
        this.slowMoveSpeed = moveSlowSpeed;
        this.firingInterval = firingInterval;
        this.chargeTime = chargeTime;
        this.killedSENumber = killedSENumber;
        this.playerBulletEntity = playerBulletEntity;
        this.playerModelEntity = playerModelEntity;
    }
}

/// <summary>
/// PL�ɕK�v�Ȑݒ�
/// </summary>
public class PlayerAuthoring : SingletonMonoBehaviour<PlayerAuthoring>
{
    [SerializeField, Min(0), Header("PL�ړ����̌X���̑傫��")]
    private int _playerMoveTilt = 0;

    [SerializeField, Min(0.0f), Header("�ړ����x")]
    private float _moveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("�������̈ړ����x")]
    private float _slowMoveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("�ˌ��Ԋu")]
    private float _firingInterval = 0.0f;

    [SerializeField, Min(0.0f), Header("�`���[�W����")]
    private float _chargeTime = 0.0f;

    [SerializeField, Min(0), Header("���S���̌��ʉ��ԍ�")]
    private int _killedSENumber = 0;

    [SerializeField, Header("PL�̒e��Prefab")]
    private Transform _playerBulletPrefab = null;

    [SerializeField, Header("PL�̌����ڂ�Entity")]
    private Transform _playerModelTransform = null;

    [SerializeField, Header("�w�c�̎��")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("Entity�̃J�e�S��")]
    private EntityCategory _entityCategory = 0;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Prefab��Entity�ɕϊ�
            var playerBulletEntity = GetEntity(src._playerBulletPrefab, TransformUsageFlags.Dynamic);
            var playerModelEntity = GetEntity(src._playerModelTransform, TransformUsageFlags.Dynamic);

            var playerData = new PlayerSingletonData
            (
                src._playerMoveTilt,
                src._moveSpeed,
                src._slowMoveSpeed,
                src._firingInterval,
                src._chargeTime,
                src._killedSENumber,
                playerBulletEntity,
                playerModelEntity
            );

            // Data���A�^�b�`
            AddComponent(entity, playerData);
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new DestroyableData());

            // �w�c�ƃJ�e�S����Tag���A�^�b�`
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src._campsType));
            AddComponent(entity, EntityCategoryHelper.GetCategoryTagType(src._entityCategory));
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static PlayerHelper;
using static TriggerHelper;
using static EntityCampsHelper;
using static EntityCategoryHelper;
using static HealthPointDatas;

#if UNITY_EDITOR
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

/// <summary>
/// PL�̏��i�V���O���g���O��j
/// </summary>
public struct PlayerSingletonData : IComponentData
{
    [Tooltip("�ړ����x")]
    public readonly float moveSpeed;

    [Tooltip("�������̈ړ����x")]
    public readonly float slowMoveSpeed;

    [Tooltip("�ˌ��Ԋu")]
    public readonly float firingInterval;

    [Tooltip("PL�̒e��PrefabEntity")]
    public readonly Entity playerBulletEntity;

    public double lastShotTime;
    public bool isSlowdown;

    /// <summary>
    /// PL�̏��
    /// </summary>
    /// <param name="moveSpeed">�ړ����x</param>
    /// <param name="moveSlowSpeed">�������̈ړ����x</param>
    /// <param name="firingInterval">�ˌ��Ԋu</param>
    /// <param name="playerBulletEntity">PL�̒e��PrefabEntity</param>
    public PlayerSingletonData(float moveSpeed, float moveSlowSpeed, float firingInterval, Entity playerBulletEntity)
    {
        this.moveSpeed = moveSpeed;
        this.slowMoveSpeed = moveSlowSpeed;
        this.firingInterval = firingInterval;
        this.playerBulletEntity = playerBulletEntity;

        lastShotTime = 0.0f;
        isSlowdown = false;
    }
}

/// <summary>
/// PL�̐ݒ�
/// </summary>
public class PlayerAuthoring : SingletonMonoBehaviour<PlayerAuthoring>
{
    [SerializeField, Min(0.0f), Header("�ړ����x")]
    private float _moveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("�������̈ړ����x")]
    private float _slowMoveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("�ő�̗�")]
    private float _maxHP = 0.0f;

    [SerializeField, Min(0.0f), Header("�ˌ��Ԋu")]
    private float _firingInterval = 0.0f;

    [SerializeField, Min(0.0f), Header("���G���Ԃ̒���")]
    private float _isInvincibleTime = 0.0f;

    [SerializeField, Min(0.0f), Header("���S���̌��ʉ��ԍ�")]
    private int _killedSENumber;

    [SerializeField, Header("PL�̒e��Prefab")]
    private Transform _playerBulletPrefab = null;

    [SerializeField, Header("�w�c�̎��")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("Entity�̃J�e�S��")]
    private EntityCategory _entityCategory = 0;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var PlayerBulletEntity = GetEntity(src._playerBulletPrefab, TransformUsageFlags.Dynamic);

            var playerData = new PlayerSingletonData
            (
                src._moveSpeed, 
                src._slowMoveSpeed,
                src._firingInterval,
                PlayerBulletEntity
            );

            // Data���A�^�b�`
            AddComponent(entity, playerData);
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new DestroyableData());
            AddComponent(entity, new PlayerHealthPointData(src._maxHP, src._isInvincibleTime, src._killedSENumber));

            // �w�c�ƃJ�e�S����Tag���A�^�b�`
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src._campsType));
            AddComponent(entity, EntityCategoryHelper.GetCategoryTagType(src._entityCategory));
        }
    }
}

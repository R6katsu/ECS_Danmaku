using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static DanmakuJobs;
using static HealthHelper;
using static PlayerAuthoring;
using static PlayerHelper;
using static TriggerHelper;
#if UNITY_EDITOR
using static UnityEngine.EventSystems.EventTrigger;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
#endif

/// <summary>
/// PL�̏��
/// </summary>
public struct PlayerData : IComponentData
{
    public readonly float moveSpeed;
    public readonly float slowMoveSpeed;
    public readonly float firingInterval;
    public readonly Entity playerBulletEntity;

    public double lastShotTime;
    public bool isSlowdown;

    /// <summary>
    /// PL�̏��
    /// </summary>
    /// <param name="moveSpeed">�ړ����x</param>
    /// <param name="moveSlowSpeed">�������̈ړ����x</param>
    /// <param name="firingInterval">�ˌ��Ԋu</param>
    /// <param name="playerBulletEntity">PL�̒e��Entity</param>
    public PlayerData(float moveSpeed, float moveSlowSpeed, float firingInterval, Entity playerBulletEntity)
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
public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField,Min(0.0f), Header("�ړ����x")]
    private float _moveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("�������̈ړ����x")]
    private float _slowMoveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("�ő�̗�")]
    private float _maxHP = 0.0f;

    [SerializeField, Min(0.0f), Header("�ˌ��Ԋu")]
    private float _firingInterval = 0.0f;

    [SerializeField, Min(0.0f), Header("���G���Ԃ̒���")]
    private float _isInvincibleTime = 0.0f;

    [SerializeField, Header("PL�̒e��Prefab")]
    private Transform _playerBulletPrefab = null;

    /// <summary>
    /// �ړ����x
    /// </summary>
    public float MoveSpeed => _moveSpeed;

    /// <summary>
    /// �������̈ړ����x
    /// </summary>
    public float SlowMoveSpeed => _slowMoveSpeed;

    /// <summary>
    /// �ő�̗�
    /// </summary>
    public float MaxHP => _maxHP;

    /// <summary>
    /// �ˌ��Ԋu
    /// </summary>
    public float FiringInterval => _firingInterval;

    /// <summary>
    /// ���G���Ԃ̒���
    /// </summary>
    public float IsInvincibleTime => _isInvincibleTime;

    /// <summary>
    /// PL�̒e��Prefab
    /// </summary>
    public Transform PlayerBulletPrefab => _playerBulletPrefab;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var PlayerBulletEntity = GetEntity(src.PlayerBulletPrefab, TransformUsageFlags.Dynamic);

            // Data���A�^�b�`
            AddComponent(entity, new PlayerData(src.MoveSpeed, src.SlowMoveSpeed, src.FiringInterval, PlayerBulletEntity));
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new DestroyableData());
            AddComponent(entity, new PlayerHealthPointData(src.MaxHP, src.IsInvincibleTime));
        }
    }
}

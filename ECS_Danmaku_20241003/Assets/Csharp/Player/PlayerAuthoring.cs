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
/// PLの情報
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
    /// PLの情報
    /// </summary>
    /// <param name="moveSpeed">移動速度</param>
    /// <param name="moveSlowSpeed">減速中の移動速度</param>
    /// <param name="firingInterval">射撃間隔</param>
    /// <param name="playerBulletEntity">PLの弾のEntity</param>
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
/// PLの設定
/// </summary>
public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField,Min(0.0f), Header("移動速度")]
    private float _moveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("減速中の移動速度")]
    private float _slowMoveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("最大体力")]
    private float _maxHP = 0.0f;

    [SerializeField, Min(0.0f), Header("射撃間隔")]
    private float _firingInterval = 0.0f;

    [SerializeField, Min(0.0f), Header("無敵時間の長さ")]
    private float _isInvincibleTime = 0.0f;

    [SerializeField, Header("PLの弾のPrefab")]
    private Transform _playerBulletPrefab = null;

    /// <summary>
    /// 移動速度
    /// </summary>
    public float MoveSpeed => _moveSpeed;

    /// <summary>
    /// 減速中の移動速度
    /// </summary>
    public float SlowMoveSpeed => _slowMoveSpeed;

    /// <summary>
    /// 最大体力
    /// </summary>
    public float MaxHP => _maxHP;

    /// <summary>
    /// 射撃間隔
    /// </summary>
    public float FiringInterval => _firingInterval;

    /// <summary>
    /// 無敵時間の長さ
    /// </summary>
    public float IsInvincibleTime => _isInvincibleTime;

    /// <summary>
    /// PLの弾のPrefab
    /// </summary>
    public Transform PlayerBulletPrefab => _playerBulletPrefab;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var PlayerBulletEntity = GetEntity(src.PlayerBulletPrefab, TransformUsageFlags.Dynamic);

            // Dataをアタッチ
            AddComponent(entity, new PlayerData(src.MoveSpeed, src.SlowMoveSpeed, src.FiringInterval, PlayerBulletEntity));
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new DestroyableData());
            AddComponent(entity, new PlayerHealthPointData(src.MaxHP, src.IsInvincibleTime));
        }
    }
}

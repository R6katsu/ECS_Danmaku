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
/// PLの情報（シングルトン前提）
/// </summary>
public struct PlayerSingletonData : IComponentData
{
    [Tooltip("最大移動可能範囲")]
    public readonly float3 maxMovementRange;

    [Tooltip("最小移動可能範囲")]
    public readonly float3 minMovementRange;

    [Tooltip("移動速度")]
    public readonly float moveSpeed;

    [Tooltip("減速中の移動速度")]
    public readonly float slowMoveSpeed;

    [Tooltip("射撃間隔")]
    public readonly float firingInterval;

    [Tooltip("PLの弾のPrefabEntity")]
    public readonly Entity playerBulletEntity;

    public double lastShotTime;
    public bool isSlowdown;

    /// <summary>
    /// PLの情報
    /// </summary>
    /// <param name="maxMovementRange">最大移動可能範囲</param>
    /// <param name="minMovementRange">最小移動可能範囲</param>
    /// <param name="moveSpeed">移動速度</param>
    /// <param name="moveSlowSpeed">減速中の移動速度</param>
    /// <param name="firingInterval">射撃間隔</param>
    /// <param name="playerBulletEntity">PLの弾のPrefabEntity</param>
    public PlayerSingletonData(float3 maxMovementRange, float3 minMovementRange, float moveSpeed, float moveSlowSpeed, float firingInterval, Entity playerBulletEntity)
    {
        this.maxMovementRange = maxMovementRange;
        this.minMovementRange = minMovementRange;
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
public class PlayerAuthoring : SingletonMonoBehaviour<PlayerAuthoring>
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        // 立方体の8つの頂点を計算
        Vector3 frontTopLeft = new Vector3(_minMovementRange.x, _maxMovementRange.y, _minMovementRange.z);
        Vector3 frontTopRight = new Vector3(_maxMovementRange.x, _maxMovementRange.y, _minMovementRange.z);
        Vector3 frontBottomLeft = new Vector3(_minMovementRange.x, _minMovementRange.y, _minMovementRange.z);
        Vector3 frontBottomRight = new Vector3(_maxMovementRange.x, _minMovementRange.y, _minMovementRange.z);

        Vector3 backTopLeft = new Vector3(_minMovementRange.x, _maxMovementRange.y, _maxMovementRange.z);
        Vector3 backTopRight = new Vector3(_maxMovementRange.x, _maxMovementRange.y, _maxMovementRange.z);
        Vector3 backBottomLeft = new Vector3(_minMovementRange.x, _minMovementRange.y, _maxMovementRange.z);
        Vector3 backBottomRight = new Vector3(_maxMovementRange.x, _minMovementRange.y, _maxMovementRange.z);

        // 前面の四辺
        Gizmos.DrawLine(frontTopLeft, frontTopRight);
        Gizmos.DrawLine(frontTopRight, frontBottomRight);
        Gizmos.DrawLine(frontBottomRight, frontBottomLeft);
        Gizmos.DrawLine(frontBottomLeft, frontTopLeft);

        // 背面の四辺
        Gizmos.DrawLine(backTopLeft, backTopRight);
        Gizmos.DrawLine(backTopRight, backBottomRight);
        Gizmos.DrawLine(backBottomRight, backBottomLeft);
        Gizmos.DrawLine(backBottomLeft, backTopLeft);

        // 前面と背面を繋ぐ辺
        Gizmos.DrawLine(frontTopLeft, backTopLeft);
        Gizmos.DrawLine(frontTopRight, backTopRight);
        Gizmos.DrawLine(frontBottomLeft, backBottomLeft);
        Gizmos.DrawLine(frontBottomRight, backBottomRight);
    }

    [SerializeField, Header("最大移動可能範囲")]
    private float3 _maxMovementRange = 0.0f;

    [SerializeField, Header("最小移動可能範囲")]
    private float3 _minMovementRange = 0.0f;

    [SerializeField, Min(0.0f), Header("移動速度")]
    private float _moveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("減速中の移動速度")]
    private float _slowMoveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("最大体力")]
    private float _maxHP = 0.0f;

    [SerializeField, Min(0.0f), Header("射撃間隔")]
    private float _firingInterval = 0.0f;

    [SerializeField, Min(0.0f), Header("無敵時間の長さ")]
    private float _isInvincibleTime = 0.0f;

    [SerializeField, Min(0.0f), Header("死亡時の効果音番号")]
    private int _killedSENumber;

    [SerializeField, Header("PLの弾のPrefab")]
    private Transform _playerBulletPrefab = null;

    [SerializeField, Header("陣営の種類")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("Entityのカテゴリ")]
    private EntityCategory _entityCategory = 0;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var PlayerBulletEntity = GetEntity(src._playerBulletPrefab, TransformUsageFlags.Dynamic);

            var playerData = new PlayerSingletonData
            (
                src._maxMovementRange, 
                src._minMovementRange, 
                src._moveSpeed, 
                src._slowMoveSpeed,
                src._firingInterval,
                PlayerBulletEntity
            );

            // Dataをアタッチ
            AddComponent(entity, playerData);
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new DestroyableData());
            AddComponent(entity, new PlayerHealthPointData(src._maxHP, src._isInvincibleTime, src._killedSENumber));

            // 陣営とカテゴリのTagをアタッチ
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src._campsType));
            AddComponent(entity, EntityCategoryHelper.GetCategoryTagType(src._entityCategory));
        }
    }
}

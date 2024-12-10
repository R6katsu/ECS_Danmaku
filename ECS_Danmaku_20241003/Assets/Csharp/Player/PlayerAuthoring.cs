using Unity.Entities;
using UnityEngine;
using static PlayerHelper;
using static EntityCategoryHelper;
using static HealthPointDatas;

#if UNITY_EDITOR
using System;
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

/// <summary>
/// PLの情報（シングルトン前提）
/// </summary>
public struct PlayerSingletonData : IComponentData
{
    [Tooltip("移動速度")]
    public readonly float moveSpeed;

    [Tooltip("減速中の移動速度")]
    public readonly float slowMoveSpeed;

    [Tooltip("射撃間隔")]
    public readonly float firingInterval;

    [Tooltip("死亡時の効果音番号")]
    public readonly int killedSENumber;

    [Tooltip("PLの弾のPrefabEntity")]
    public readonly Entity playerBulletEntity;

    [Tooltip("PLの見た目のEntity")]
    public readonly Entity playerModelEntity;

    public double lastShotTime;
    public bool isSlowdown;

    /// <summary>
    /// PLの情報
    /// </summary>
    /// <param name="moveSpeed">移動速度</param>
    /// <param name="moveSlowSpeed">減速中の移動速度</param>
    /// <param name="firingInterval">射撃間隔</param>
    /// <param name="killedSENumber">死亡時の効果音番号</param>
    /// <param name="playerBulletEntity">PLの弾のPrefabEntity</param>
    public PlayerSingletonData
        (
        float moveSpeed,
        float moveSlowSpeed, 
        float firingInterval,
        int killedSENumber,
        Entity playerBulletEntity,
        Entity playerModelEntity
        )
    {
        this.moveSpeed = moveSpeed;
        this.slowMoveSpeed = moveSlowSpeed;
        this.firingInterval = firingInterval;
        this.killedSENumber = killedSENumber;
        this.playerBulletEntity = playerBulletEntity;
        this.playerModelEntity = playerModelEntity;

        lastShotTime = 0.0f;
        isSlowdown = false;
    }
}

/// <summary>
/// PLの設定
/// </summary>
public class PlayerAuthoring : SingletonMonoBehaviour<PlayerAuthoring>
{
    [SerializeField, Min(0.0f), Header("移動速度")]
    private float _moveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("減速中の移動速度")]
    private float _slowMoveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("射撃間隔")]
    private float _firingInterval = 0.0f;

    [SerializeField, Min(0), Header("死亡時の効果音番号")]
    private int _killedSENumber = 0;

    [SerializeField, Header("PLの弾のPrefab")]
    private Transform _playerBulletPrefab = null;

    [SerializeField, Header("PLの見た目のEntity")]
    private Transform _playerModelTransform = null;

    [SerializeField, Header("陣営の種類")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("Entityのカテゴリ")]
    private EntityCategory _entityCategory = 0;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var playerBulletEntity = GetEntity(src._playerBulletPrefab, TransformUsageFlags.Dynamic);
            var playerModelEntity = GetEntity(src._playerModelTransform, TransformUsageFlags.Dynamic);

            var playerData = new PlayerSingletonData
            (
                src._moveSpeed, 
                src._slowMoveSpeed,
                src._firingInterval,
                src._killedSENumber,
                playerBulletEntity,
                playerModelEntity
            );

            // Dataをアタッチ
            AddComponent(entity, playerData);
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new DestroyableData());

            // 陣営とカテゴリのTagをアタッチ
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src._campsType));
            AddComponent(entity, EntityCategoryHelper.GetCategoryTagType(src._entityCategory));
        }
    }
}

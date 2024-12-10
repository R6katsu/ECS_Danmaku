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

// リファクタリング済み

/// <summary>
/// PLの情報（シングルトン前提）
/// </summary>
public struct PlayerSingletonData : IComponentData
{
    [Tooltip("PL移動時の傾きの大きさ")]
    public readonly int playerMoveTilt;

    [Tooltip("移動速度")]
    public readonly float moveSpeed;

    [Tooltip("減速中の移動速度")]
    public readonly float slowMoveSpeed;

    [Tooltip("射撃間隔")]
    public readonly float firingInterval;

    [Tooltip("チャージ時間")]
    public readonly float chargeTime;

    [Tooltip("死亡時の効果音番号")]
    public readonly int killedSENumber;

    [Tooltip("PLの弾のPrefabEntity")]
    public readonly Entity playerBulletEntity;

    [Tooltip("PLの見た目のEntity")]
    public readonly Entity playerModelEntity;

    /// <summary>
    /// PLの情報
    /// </summary>
    /// <param name="playerMoveTilt">移動速度</param>
    /// <param name="moveSpeed">移動速度</param>
    /// <param name="moveSlowSpeed">減速中の移動速度</param>
    /// <param name="firingInterval">射撃間隔</param>
    /// <param name="chargeTime">チャージ時間</param>
    /// <param name="killedSENumber">死亡時の効果音番号</param>
    /// <param name="playerBulletEntity">PLの弾のPrefabEntity</param>
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
/// PLに必要な設定
/// </summary>
public class PlayerAuthoring : SingletonMonoBehaviour<PlayerAuthoring>
{
    [SerializeField, Min(0), Header("PL移動時の傾きの大きさ")]
    private int _playerMoveTilt = 0;

    [SerializeField, Min(0.0f), Header("移動速度")]
    private float _moveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("減速中の移動速度")]
    private float _slowMoveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("射撃間隔")]
    private float _firingInterval = 0.0f;

    [SerializeField, Min(0.0f), Header("チャージ時間")]
    private float _chargeTime = 0.0f;

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

            // PrefabをEntityに変換
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

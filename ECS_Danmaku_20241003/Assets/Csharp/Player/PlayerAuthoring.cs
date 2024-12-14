using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System;
using static EntityCategoryHelper;
using static HealthPointDatas;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Transforms;
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

    [Tooltip("チャージ完了までの時間")]
    public readonly float chargeTime;

    [Tooltip("チャージが完了した効果音")]
    public readonly int chargeFinishedSE;

    [Tooltip("チャージショットの効果音")]
    public readonly int chargeShotSE;

    [Tooltip("チャージショットが不発だった時の効果音")]
    public readonly int chargeShotMissSE;

    [Tooltip("PLの弾のPrefabEntity")]
    public readonly Entity playerBulletEntity;

    [Tooltip("チャージ弾のPrefabEntity")]
    public readonly Entity chargeBulletEntity;

    [Tooltip("PLの見た目のEntity")]
    public readonly Entity playerModelEntity;

    /// <summary>
    /// PLの情報
    /// </summary>
    /// <param name="playerMoveTilt">移動速度</param>
    /// <param name="moveSpeed">移動速度</param>
    /// <param name="slowMoveSpeed">減速中の移動速度</param>
    /// <param name="firingInterval">射撃間隔</param>
    /// <param name="chargeTime">チャージ完了までの時間</param>
    /// <param name="chargeFinishedSE">チャージが完了した効果音</param>
    /// <param name="chargeShotSE">チャージが完了した効果音</param>
    /// <param name="chargeShotMissSE">チャージショットが不発だった時の効果音</param>
    /// <param name="playerBulletEntity">PLの弾のPrefabEntity</param>
    /// <param name="maxChargeBulletEntity">チャージ弾のPrefabEntity</param>
    /// <param name="playerModelEntity">PLの見た目のEntity</param>
    public PlayerSingletonData
        (
        int playerMoveTilt,
        float moveSpeed,
        float slowMoveSpeed,
        float firingInterval,
        float chargeTime,
        int chargeFinishedSE,
        int chargeShotSE,
        int chargeShotMissSE,
        Entity playerBulletEntity,
        Entity maxChargeBulletEntity,
        Entity playerModelEntity
        )
    {
        this.playerMoveTilt = playerMoveTilt;
        this.moveSpeed = moveSpeed;
        this.slowMoveSpeed = slowMoveSpeed;
        this.firingInterval = firingInterval;
        this.chargeTime = chargeTime;
        this.chargeFinishedSE = chargeFinishedSE;
        this.chargeShotSE = chargeShotSE;
        this.chargeShotMissSE = chargeShotMissSE;
        this.playerBulletEntity = playerBulletEntity;
        this.chargeBulletEntity = maxChargeBulletEntity;
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

    [SerializeField, Min(0.0f), Header("チャージ完了までの時間")]
    private float _chargeTime = 0.0f;

    [SerializeField, Min(0), Header("チャージが完了した効果音")]
    private int _chargeFinishedSE = 0;

    [SerializeField, Min(0), Header("チャージショットの効果音")]
    private int _chargeShotSE = 0;

    [SerializeField, Min(0), Header("チャージショットが不発だった時の効果音")]
    private int _chargeShotMissSE = 0;

    [SerializeField, Header("PLの弾のPrefab")]
    private Transform _playerBulletPrefab = null;

    [SerializeField, Header("チャージ弾のPrefab")]
    private Transform _chargeBulletPrefab;

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
            var chargeBulletEntity = GetEntity(src._chargeBulletPrefab, TransformUsageFlags.Dynamic);
            var playerModelEntity = GetEntity(src._playerModelTransform, TransformUsageFlags.Dynamic);

            var playerData = new PlayerSingletonData
            (
                src._playerMoveTilt,
                src._moveSpeed,
                src._slowMoveSpeed,
                src._firingInterval,
                src._chargeTime,
                src._chargeFinishedSE,
                src._chargeShotSE,
                src._chargeShotMissSE,
                playerBulletEntity,
                chargeBulletEntity,
                playerModelEntity
            );

            // Dataをアタッチ
            AddComponent(entity, playerData);
            AddComponent(entity, new DestroyableData());

            // 陣営とカテゴリのTagをアタッチ
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src._campsType));
            AddComponent(entity, EntityCategoryHelper.GetCategoryTagType(src._entityCategory));
        }
    }
}

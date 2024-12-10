using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using System;

using Random = UnityEngine.Random;

#if UNITY_EDITOR
using Unity.Collections;
using static HealthPointDatas;
using static BulletHelper;
using static EntityCampsHelper;
using static UnityEngine.RuleTile.TilingRuleOutput;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
#endif

// リファクタリング済み

/// <summary>
/// ボス敵の処理
/// </summary>
public partial class BossEnemySystem : SystemBase
{
    /// <summary>
    /// ボス敵の状態
    /// </summary>
    private enum BossEnemyState : byte
    {
        [Tooltip("初期状態")] None,
        [Tooltip("PL狙いのn-Way弾")] PlayerTrackingNWay,
        [Tooltip("回転n-Way弾")] RotationNWay,
        [Tooltip("ランダムに弾をばら撒く")] RandomSpreadBullets
    }

    [Tooltip("円周のラジアン値")]
    private const float FULL_CIRCLE_RADIANS = Mathf.PI * 2;

    [Tooltip("ボス敵の状態")]
    private BossEnemyState _bossEnemyState = BossEnemyState.None;

    [Tooltip("前回のボス敵の状態")]
    private BossEnemyState _lastBossEnemyState = BossEnemyState.None;

    [Tooltip("現在の攻撃が切り替わるまでの時間")]
    private float _currentAttackSwitchTime = 0.0f;

    private EntityCommandBufferSystem _ecbSystem = null;

    /// <summary>
    /// ボス敵の状態
    /// </summary>
    private BossEnemyState MyBossEnemyState
    {
        get => _bossEnemyState;
        set
        {
            // 前回から変更があった
            if (_bossEnemyState != value)
            {
                // 前回のBossEnemyStateとして保持
                _lastBossEnemyState = MyBossEnemyState;

                // 反映
                _bossEnemyState = value;

                // BossEnemyStateに対応する処理
                ChangeBossEnemyState(_bossEnemyState);
            }
        }
    }

    protected override void OnCreate()
    {
        // EndSimulationEntityCommandBufferSystemで最後に反映
        _ecbSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // BossEnemySingletonDataが存在していなかった
        if (!SystemAPI.HasSingleton<BossEnemySingletonData>())
        {
            // 初期化
            Initialize();
            return;
        }

        // シングルトンデータの取得
        var bossEnemySingleton = SystemAPI.GetSingleton<BossEnemySingletonData>();

        // Entityを取得
        Entity entity = SystemAPI.GetSingletonEntity<BossEnemySingletonData>();

        // 移動中だった
        if (SystemAPI.HasComponent<MoveToTargetPointData>(entity)) { return; }

        // 0以下になったら次の攻撃へ切り替える
        _currentAttackSwitchTime -= SystemAPI.Time.DeltaTime;
        if (_currentAttackSwitchTime <= 0.0f)
        {
            // 攻撃が切り替わるまでの時間を最大値へ回復
            _currentAttackSwitchTime = bossEnemySingleton.attackSwitchTime;

            // BossEnemyStateに対応するリセット処理
            ResetBossEnemyState(_bossEnemyState);

            // 1フレーム待機の為にNoneを経由
            MyBossEnemyState = BossEnemyState.None;
            return;
        }

        switch (MyBossEnemyState)
        {
            case BossEnemyState.PlayerTrackingNWay:
                // PL狙いのn-Way弾
                PlayerTrackingNWay();
                break;

            case BossEnemyState.RotationNWay:
                // 回転n-Way弾
                RotationNWay();
                break;

            case BossEnemyState.RandomSpreadBullets:
                // ランダムに弾をばら撒く
                RandomSpreadBullets();
                break;

            case BossEnemyState.None:
            default:
                // 前回の攻撃と同じだったら変更しない
                var bossEnemyState = GetRandomBossEnemyState();
                MyBossEnemyState = (_lastBossEnemyState == bossEnemyState) ? MyBossEnemyState : bossEnemyState;
                break;
        }
    }

    /// <summary>
    /// 現在のBossEnemyStateに対応するリセット処理
    /// </summary>
    private void ResetBossEnemyState(BossEnemyState bossEnemyState)
    {
        // BossEnemySingletonDataが存在していなかった
        if (!SystemAPI.HasSingleton<BossEnemySingletonData>()) { return; }

        // シングルトンデータの取得
        var bossEnemySingleton = SystemAPI.GetSingleton<BossEnemySingletonData>();

        // Entityを取得
        Entity entity = SystemAPI.GetSingletonEntity<BossEnemySingletonData>();

        EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();

        switch (bossEnemyState)
        {
            case BossEnemyState.PlayerTrackingNWay:
                // N_Way_DanmakuDataを有していた
                if (SystemAPI.HasComponent<NWay_DanmakuData>(entity))
                {
                    ecb.RemoveComponent<NWay_DanmakuData>(entity);
                }
                break;

            case BossEnemyState.RotationNWay:
                // RotationDataを有していた
                if (SystemAPI.HasComponent<RotationData>(entity))
                {
                    ecb.RemoveComponent<RotationData>(entity);
                }

                // N_Way_DanmakuDataを有していた
                if (SystemAPI.HasComponent<NWay_DanmakuData>(entity))
                {
                    ecb.RemoveComponent<NWay_DanmakuData>(entity);
                }
                break;

            case BossEnemyState.RandomSpreadBullets:
                // N_Way_DanmakuDataを有していた
                if (SystemAPI.HasComponent<NWay_DanmakuData>(entity))
                {
                    ecb.RemoveComponent<NWay_DanmakuData>(entity);
                }
                break;

            case BossEnemyState.None:
            default:
                break;
        }

        // フレーム終了時に適用
        _ecbSystem.AddJobHandleForProducer(Dependency);
    }

    /// <summary>
    /// BossEnemyStateに対応する処理
    /// </summary>
    private void ChangeBossEnemyState(BossEnemyState bossEnemyState)
    {
        // BossEnemySingletonDataが存在していなかった
        if (!SystemAPI.HasSingleton<BossEnemySingletonData>()) { return; }

        // シングルトンデータの取得
        var bossEnemySingleton = SystemAPI.GetSingleton<BossEnemySingletonData>();

        // Entityを取得
        Entity entity = SystemAPI.GetSingletonEntity<BossEnemySingletonData>();

        switch (bossEnemyState)
        {
            case BossEnemyState.PlayerTrackingNWay:
                // NWay_DanmakuDataを有していなかった
                if (!SystemAPI.HasComponent<NWay_DanmakuData>(entity))
                {
                    // アタッチ
                    EntityManager.AddComponent<NWay_DanmakuData>(entity);

                    var nWay_DanmakuData = new NWay_DanmakuData
                    (
                        bossEnemySingleton.playerTrackingNWay.fanAngle,
                        bossEnemySingleton.playerTrackingNWay.amountBullets,
                        bossEnemySingleton.playerTrackingNWay.firingInterval,
                        bossEnemySingleton.playerTrackingNWay.bulletEntity
                    );

                    // 変数を設定
                    EntityManager.SetComponentData(entity, nWay_DanmakuData);
                }
                break;

            case BossEnemyState.RotationNWay:
                // RotationDataを有していなかった
                if (!SystemAPI.HasComponent<RotationData>(entity))
                {
                    // アタッチ
                    EntityManager.AddComponent<RotationData>(entity);

                    var rotationData = new RotationData
                    (
                        bossEnemySingleton.rotation.AxisType,
                        bossEnemySingleton.rotation.RotationSpeed
                    );

                    // 変数を設定
                    EntityManager.SetComponentData(entity, rotationData);
                }

                // NWay_DanmakuDataを有していなかった
                if (!SystemAPI.HasComponent<NWay_DanmakuData>(entity))
                {
                    // アタッチ
                    EntityManager.AddComponent<NWay_DanmakuData>(entity);

                    var nWay_DanmakuData = new NWay_DanmakuData
                    (
                        bossEnemySingleton.rotationNWay.fanAngle,
                        bossEnemySingleton.rotationNWay.amountBullets,
                        bossEnemySingleton.rotationNWay.firingInterval,
                        bossEnemySingleton.rotationNWay.bulletEntity
                    );

                    // 変数を設定
                    EntityManager.SetComponentData(entity, nWay_DanmakuData);
                }

                break;

            case BossEnemyState.RandomSpreadBullets:
                // NWay_DanmakuDataを有していなかった
                if (!SystemAPI.HasComponent<NWay_DanmakuData>(entity))
                {
                    // アタッチ
                    EntityManager.AddComponent<NWay_DanmakuData>(entity);

                    var nWay_DanmakuData = new NWay_DanmakuData
                    (
                        bossEnemySingleton.randomSpreadBulletsNWay.fanAngle,
                        bossEnemySingleton.randomSpreadBulletsNWay.amountBullets,
                        bossEnemySingleton.randomSpreadBulletsNWay.firingInterval,
                        bossEnemySingleton.randomSpreadBulletsNWay.bulletEntity
                    );

                    // 変数を設定
                    EntityManager.SetComponentData(entity, nWay_DanmakuData);
                }
                break;

            case BossEnemyState.None:
            default:
                break;
        }
    }

    /// <summary>
    /// ランダムにBossEnemyStateを抽選
    /// </summary>
    /// <returns>抽選したBossEnemyState</returns>
    private BossEnemyState GetRandomBossEnemyState()
    {
        // BossEnemyStateの要素を全て取得
        Array bossEnemyStateArray = Enum.GetValues(typeof(BossEnemyState));

        // 初期状態を除外
        BossEnemyState[] validStates = Array.FindAll
        (
            bossEnemyStateArray as BossEnemyState[],
            state => state != BossEnemyState.None
        );

        // 抽選して返す
        return validStates[Random.Range(0, validStates.Length)];
    }

    /// <summary>
    /// PL狙いのn-Way弾
    /// </summary>
    private void PlayerTrackingNWay()
    {
        // BossEnemySingletonDataが存在していなかった
        if (!SystemAPI.HasSingleton<BossEnemySingletonData>()) { return; }

        // シングルトンデータの取得
        var bossEnemySingleton = SystemAPI.GetSingleton<BossEnemySingletonData>();

        // Entityを取得
        Entity bossEntity = SystemAPI.GetSingletonEntity<BossEnemySingletonData>();

        // PlayerSingletonDataが存在していなかった
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // シングルトンデータの取得
        var playerSingleton = SystemAPI.GetSingleton<PlayerSingletonData>();

        // Entityを取得
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerSingletonData>();

        // LocalTransformを所持していなかった
        if (!SystemAPI.HasComponent<LocalTransform>(playerEntity)) { return; }

        // LocalTransformを取得
        var playerTfm = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        // LocalTransformを所持していなかった
        if (!SystemAPI.HasComponent<LocalTransform>(bossEntity)) { return; }

        // LocalTransformを取得
        var bossTfm = SystemAPI.GetComponent<LocalTransform>(bossEntity);

        // 回転を適用
        Quaternion targetRotation = Quaternion.LookRotation(playerTfm.Position - bossTfm.Position);
        bossTfm.Rotation = targetRotation;
        EntityManager.SetComponentData(bossEntity, bossTfm);
    }

    /// <summary>
    /// 回転しながらn-Way弾
    /// </summary>
    private void RotationNWay()
    {
#if UNITY_EDITOR
        Debug.Log("回転しながらn-Way弾");
#endif
    }

    /// <summary>
    /// ランダムに弾をばら撒く
    /// </summary>
    private void RandomSpreadBullets()
    {
        // BossEnemySingletonDataが存在していなかった
        if (!SystemAPI.HasSingleton<BossEnemySingletonData>()) { return; }

        // シングルトンデータの取得
        var bossEnemySingleton = SystemAPI.GetSingleton<BossEnemySingletonData>();

        // Entityを取得
        Entity entity = SystemAPI.GetSingletonEntity<BossEnemySingletonData>();

        // LocalTransformを所持していなかった
        if (!SystemAPI.HasComponent<LocalTransform>(entity)) { return; }

        // LocalTransformを取得
        var localTfm = SystemAPI.GetComponent<LocalTransform>(entity);

        // 円周のランダムな位置を計算
        float theta = Random.Range(0f, FULL_CIRCLE_RADIANS);    // 0度から360度までのランダムな角度
        float x = Mathf.Cos(theta);                             // Cos値を計算（余弦）
        float y = 0.0f;                                         // 高さを定義
        float z = Mathf.Sin(theta);                             // Sin値を計算（正弦）
        Vector3 randomDirection = new Vector3(x, y, z);

        // 回転を適用
        Quaternion targetRotation = Quaternion.LookRotation(randomDirection);
        localTfm.Rotation = targetRotation;
        EntityManager.SetComponentData(entity, localTfm);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        MyBossEnemyState = BossEnemyState.None;
    }
}

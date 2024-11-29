using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Unity.Collections;


#if UNITY_EDITOR
using static UnityEngine.RuleTile.TilingRuleOutput;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
#endif

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

    [Tooltip("ボス敵の状態")]
    private BossEnemyState _bossEnemyState = BossEnemyState.None;

    [Tooltip("攻撃が切り替わるまでの時間")]
    private float _attackSwitchTime = 10.0f;     // インスペクタから設定できるようにする

    [Tooltip("現在の攻撃が切り替わるまでの時間")]
    private float _currentAttackSwitchTime = 0.0f;

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
                // 攻撃が切り替わるまでの時間を最大値へ回復
                _currentAttackSwitchTime = _attackSwitchTime;

                // BossEnemyStateに対応するリセット処理
                ResetBossEnemyState(_bossEnemyState);

                // 反映
                _bossEnemyState = value;

                // BossEnemyStateに対応する処理
                ChangeBossEnemyState(_bossEnemyState);
            }
        }
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

        // 0以下になったら次の攻撃へ切り替える
        _currentAttackSwitchTime -= SystemAPI.Time.DeltaTime;
        if (_currentAttackSwitchTime <= 0.0f)
        {
            MyBossEnemyState = GetRandomBossEnemyState();
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
                MyBossEnemyState = GetRandomBossEnemyState();
                break;
        }
    }

    /// <summary>
    /// 現在のBossEnemyStateに対応するリセット処理
    /// </summary>
    private void ResetBossEnemyState(BossEnemyState bossEnemyState)
    {
        // nWayとばら撒き相性良さげ



        // BossEnemySingletonDataが存在していなかった
        if (!SystemAPI.HasSingleton<BossEnemySingletonData>()) { return; }

        // シングルトンデータの取得
        var bossEnemySingleton = SystemAPI.GetSingleton<BossEnemySingletonData>();

        // Entityを取得
        Entity entity = SystemAPI.GetSingletonEntity<BossEnemySingletonData>();

        switch (bossEnemyState)
        {
            case BossEnemyState.PlayerTrackingNWay:
                // N_Way_DanmakuDataを有していた
                if (SystemAPI.HasComponent<N_Way_DanmakuData>(entity))
                {
                    // N_Way_DanmakuDataを取得
                    var nWay_DanmakuData = SystemAPI.GetComponent<N_Way_DanmakuData>(entity);

                    nWay_DanmakuData.IsDataDeletion = true;
                }
                break;

            case BossEnemyState.RotationNWay:
                // RotationDataを有していた
                if (SystemAPI.HasComponent<RotationData>(entity))
                {
                    // RotationDataを取得
                    var rotationData = SystemAPI.GetComponent<RotationData>(entity);

                    rotationData.IsDataDeletion = true;
                }

                // N_Way_DanmakuDataを有していた
                if (SystemAPI.HasComponent<N_Way_DanmakuData>(entity))
                {
                    // N_Way_DanmakuDataを取得
                    var nWay_DanmakuData = SystemAPI.GetComponent<N_Way_DanmakuData>(entity);

                    nWay_DanmakuData.IsDataDeletion = true;
                }
                break;

            case BossEnemyState.RandomSpreadBullets:
                // TapShooting_DanmakuDataを有していた
                if (SystemAPI.HasComponent<TapShooting_DanmakuData>(entity))
                {
                    // TapShooting_DanmakuDataを取得
                    var tapShooting_DanmakuData = SystemAPI.GetComponent<TapShooting_DanmakuData>(entity);

                    tapShooting_DanmakuData.IsDataDeletion = true;
                }
                break;

            case BossEnemyState.None:
            default:
                break;
        }
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
                // N_Way_DanmakuDataを有していなかった
                if (!SystemAPI.HasComponent<N_Way_DanmakuData>(entity))
                {
                    // アタッチ
                    EntityManager.AddComponent<N_Way_DanmakuData>(entity);

                    Debug.LogError("マジックナンバー。インスペクタから設定する際の方法を考える");

                    var nWay_DanmakuData = new N_Way_DanmakuData
                    (
                        120,
                        12,
                        1f,
                        bossEnemySingleton.nWayBulletEntity,
                        bossEnemySingleton.bulletLocalScale
                    );

                    // 変数を設定
                    EntityManager.SetComponentData<N_Way_DanmakuData>(entity, nWay_DanmakuData);
                }
                break;

            case BossEnemyState.RotationNWay:
                // RotationDataを有していなかった
                if (!SystemAPI.HasComponent<RotationData>(entity))
                {
                    // アタッチ
                    EntityManager.AddComponent<RotationData>(entity);

                    var rotationData = new RotationData()
                    {
                        axisType = AxisType.Y,
                        rotationSpeed = bossEnemySingleton.rotationSpeed
                    };

                    // 変数を設定
                    EntityManager.SetComponentData<RotationData>(entity, rotationData);
                }

                // N_Way_DanmakuDataを有していなかった
                if (!SystemAPI.HasComponent<N_Way_DanmakuData>(entity))
                {
                    // アタッチ
                    EntityManager.AddComponent<N_Way_DanmakuData>(entity);

                    var nWay_DanmakuData = new N_Way_DanmakuData
                    (
                        bossEnemySingleton.fanAngle,
                        bossEnemySingleton.amountBullets,
                        bossEnemySingleton.nWayFiringInterval,
                        bossEnemySingleton.nWayBulletEntity,
                        bossEnemySingleton.bulletLocalScale
                    );

                    // 変数を設定
                    EntityManager.SetComponentData<N_Way_DanmakuData>(entity, nWay_DanmakuData);
                }

                break;

            case BossEnemyState.RandomSpreadBullets:
                // TapShooting_DanmakuDataを有していなかった
                if (!SystemAPI.HasComponent<TapShooting_DanmakuData>(entity))
                {
                    // アタッチ
                    EntityManager.AddComponent<TapShooting_DanmakuData>(entity);

                    var tapShooting_DanmakuData = new TapShooting_DanmakuData
                    (
                        bossEnemySingleton.shootNSingleSet,
                        bossEnemySingleton.singleSetRestTimeAfter,
                        bossEnemySingleton.tapFiringInterval,
                        bossEnemySingleton.tapBulletEntity
                    );

                    // 変数を設定
                    EntityManager.SetComponentData<TapShooting_DanmakuData>(entity, tapShooting_DanmakuData);
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
        Array allStates = Enum.GetValues(typeof(BossEnemyState));

        // 初期状態を除外
        BossEnemyState[] validStates = Array.FindAll(
            allStates as BossEnemyState[],
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
        // なんか物足りない。簡単に避けられる
        // ボス敵自体が縦横無尽に移動しながらばら撒いたら凶悪なのでは？

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
        float theta = Random.Range(0f, Mathf.PI * 2);   // 0度から360度までのランダムな角度
        float x = Mathf.Cos(theta);                     // Cos値を計算（余弦）
        float y = 0.0f;                                 // 高さを定義
        float z = Mathf.Sin(theta);                     // Sin値を計算（正弦）
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

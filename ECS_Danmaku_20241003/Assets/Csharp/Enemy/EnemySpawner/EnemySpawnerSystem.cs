using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using Random = UnityEngine.Random;
using static EntityCampsHelper;


#if UNITY_EDITOR
using Unity.Physics;
using static SpawnPointSingletonData;
using System.Linq;
using System.Linq.Expressions;
using Unity.Jobs;
using System;
using System.Collections;
using System.Collections.Generic;
using static MoveHelper;
using UnityEngine.Rendering;
using static UnityEngine.EventSystems.EventTrigger;
#endif

// リファクタリング済み

/// <summary>
/// 敵の生成処理
/// </summary>
[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    [Tooltip("大きさの初期値")]
    private const float DEFAULT_SCALE = 1.0f;

    [Tooltip("未設定の値")]
    private const int UNSET_VALUE = int.MinValue;

    private EnemySpawnPattern _currentPattern;
    private int _currentInfoNumber;
    private float _elapsed;

    [Tooltip("ボス生成までのカウントダウン")]
    private int _countdownBossSpawn;

    public void OnUpdate(ref SystemState state)
    {
        // EnemySpawnPatternArraySingletonDataが存在していなかった
        if (!SystemAPI.HasSingleton<EnemySpawnPatternArraySingletonData>())
        {
            // 初期化
            Initialize();
        }

        // 経過時間にフレーム秒を加算代入
        _elapsed += SystemAPI.Time.DeltaTime;

        // 一時的なBufferを作成
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // 敵が存在するか
        bool hasEnemy = HasEnemy(ref state);

        // EntityとDataを取得する
        foreach (var (array, entity)
            in SystemAPI.Query<EnemySpawnPatternArraySingletonData>()
            .WithEntityAccess())
        {
            // パターンの数が0以下だった
            if (array.enemySpawnPatterns.Length <= 0)
            {
#if UNITY_EDITOR
                Debug.LogError("敵を生成するパターンが存在しない");
#endif
                continue;
            }

            // ボス生成までのカウントダウンが未設定なら初期化
            _countdownBossSpawn = (_countdownBossSpawn == UNSET_VALUE) ? array.countdownBossSpawn : _countdownBossSpawn;

            // _currentPatternがdefault、
            // またはenemySpawnInfosの長さを_currentInfoNumberが超過していたら初期化
            if (_currentPattern.Equals(default) 
                || _currentPattern.enemySpawnInfos.Length <= _currentInfoNumber)
            {
                // EnemyTagを持つEntityが1体以上存在する場合はコンテニュー
                if (hasEnemy) { continue; }

                // 次に生成する敵パターンを抽選
                var patternNumber = Random.Range(0, array.enemySpawnPatterns.Length);
                _currentPattern = array.enemySpawnPatterns[patternNumber];

                // ボス生成までのカウントダウンをデクリメント
                _countdownBossSpawn--;

                // 初期化
                _currentInfoNumber = 0;
                _elapsed = 0.0f;
            }

            // ボス生成までのカウントダウンが 0以下になった
            if (_countdownBossSpawn <= 0)
            {
                // ボス敵を生成
                BossEnemyInstantiate(ref state, ecb, array);

                // EnemySpawnPatternArraySingletonDataが存在していた
                if (SystemAPI.HasSingleton<EnemySpawnPatternArraySingletonData>())
                {
                    // EnemySpawnPatternArraySingletonDataをアタッチされたEntityから削除する
                    var enemySpawnPatternArraySingletonDataEntity = SystemAPI.GetSingletonEntity<EnemySpawnPatternArraySingletonData>();
                    ecb.RemoveComponent<EnemySpawnPatternArraySingletonData>(enemySpawnPatternArraySingletonDataEntity);
                }
                break;
            }

            // 敵を生成
            EnemyInstantiate(ref state, ecb, array);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    /// <summary>
    /// EnemyNameに対応する敵Entityを取得する
    /// </summary>
    /// <param name="enemyName">敵Entityの名称</param>
    /// <returns>EnemyNameに対応する敵Entity</returns>
    private Entity GetEnemyEntity(ref SystemState systemState, EnemyName enemyName)
    {
        foreach (var enemyEntityData in SystemAPI.Query<EnemyEntityData>())
        {
            if (enemyEntityData.enemyName == enemyName)
            {
                return enemyEntityData.enemyEntity;
            }
        }

        // 見つからなかった
        return Entity.Null;
    }

    /// <summary>
    /// 敵が存在するかのフラグを返す
    /// </summary>
    /// <returns>敵が存在するか</returns>
    private bool HasEnemy(ref SystemState state)
    {
        foreach (var enemyTag in SystemAPI.Query<EnemyCampsTag>())
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// ボス敵を生成
    /// </summary>
    /// <param name="state">SystemAPIに必要</param>
    /// <param name="ecb">変更の反映に必要</param>
    /// <param name="array">敵生成に必要なもの</param>
    private void BossEnemyInstantiate(ref SystemState state, EntityCommandBuffer ecb, EnemySpawnPatternArraySingletonData array)
    {
        // ボスを生成
        var bossEnemy = ecb.Instantiate(array.bossEnemyEntity);

        // SpawnPointSingletonDataが存在していた
        if (SystemAPI.HasSingleton<SpawnPointSingletonData>())
        {
            // シングルトンデータの取得
            var spawnPointSingleton = SystemAPI.GetSingleton<SpawnPointSingletonData>();

            // 生成位置を取得
            var spawnPoint = spawnPointSingleton.GetSpawnPoint(SpawnPointType.Center);

            // nullチェック。nullだったら原点に生成
            spawnPoint = (spawnPoint == null) ? float3.zero : spawnPoint;

            // LocalTransformを設定する
            ecb.SetComponent(bossEnemy, new LocalTransform
            {
                Position = (float3)spawnPoint,
                Rotation = quaternion.identity,
                Scale = DEFAULT_SCALE
            });
        }
    }

    /// <summary>
    /// 敵を生成
    /// </summary>
    /// <param name="state">SystemAPIに必要</param>
    /// <param name="ecb">変更の反映に必要</param>
    /// <param name="array">敵生成に必要なもの</param>
    private void EnemyInstantiate(ref SystemState state, EntityCommandBuffer ecb, EnemySpawnPatternArraySingletonData array)
    {
        // 現在のenemySpawnInfoを取得
        var currentInfo = _currentPattern.enemySpawnInfos[_currentInfoNumber];

        // 経過時間が敵生成時間未満だったらコンテニュー
        if (currentInfo.CreationDelay > _elapsed) { return; }

        // 生成中の敵番号をインクリメント
        _currentInfoNumber++;

        // EnemyNameに対応する敵Entityを取得
        var enemyEntity = GetEnemyEntity(ref state, currentInfo.MyEnemyName);

        // 敵EntityがNullだったらコンテニュー
        if (enemyEntity == Entity.Null) { return; }

        // 敵を生成
        var enemy = ecb.Instantiate(enemyEntity);

        // SpawnPointSingletonDataが存在していた
        if (SystemAPI.HasSingleton<SpawnPointSingletonData>())
        {
            // シングルトンデータの取得
            var spawnPointSingleton = SystemAPI.GetSingleton<SpawnPointSingletonData>();

            // 生成位置を取得
            var spawnPoint = spawnPointSingleton.GetSpawnPoint
            (
                currentInfo.SpawnPointType
            );

            // nullチェック。nullだったら原点に生成
            spawnPoint = (spawnPoint == null) ? float3.zero : spawnPoint;

            // LocalTransformを設定する
            ecb.SetComponent(enemy, new LocalTransform
            {
                Position = (float3)spawnPoint,
                Rotation = quaternion.identity,
                Scale = DEFAULT_SCALE
            });
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        _currentPattern = new();
        _elapsed = 0.0f;
        _currentInfoNumber = 0;
        _countdownBossSpawn = UNSET_VALUE;
    }
}

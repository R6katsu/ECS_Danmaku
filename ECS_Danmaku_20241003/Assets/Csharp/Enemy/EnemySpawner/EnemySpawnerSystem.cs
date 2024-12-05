using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static EnemyHelper;

using Random = UnityEngine.Random;
using static SpawnPointSingletonData;


#if UNITY_EDITOR
using System.Linq.Expressions;
using Unity.Jobs;
using System;
using System.Collections;
using System.Collections.Generic;
using static MoveHelper;
using UnityEngine.Rendering;
using static UnityEngine.EventSystems.EventTrigger;
#endif

/// <summary>
/// 敵の生成処理
/// </summary>
[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    private float _elapsed;

    EnemySpawnPattern currentPattern;
    int currentInfoNumber;

    int bossNumber;

    [Tooltip("EnemySpawnerSystemが有効か")]
    public bool isSelfEnable;

    public void OnCreate(ref SystemState state)
    {
        Initialize();
    }

    public void OnUpdate(ref SystemState state)
    {
        // EnemySpawnPatternArraySingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<EnemySpawnPatternArraySingletonData>())
        {
            // 初期化
            Initialize();
        }

        _elapsed += SystemAPI.Time.DeltaTime;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Entityと一緒にDataを取得する
        foreach (var (array, entity) in SystemAPI.Query<EnemySpawnPatternArraySingletonData>().WithEntityAccess())
        {
            if (currentPattern.Equals(default) || currentPattern.infos.Length <= currentInfoNumber)
            {
                var patternNumber = Random.Range(0, array.infos.Length);

                currentPattern = array.infos[patternNumber];

                currentInfoNumber = 0;
                _elapsed = 0.0f;
                bossNumber--;
            }

            // 0以下になった
            if (bossNumber <= 0)
            {
                if (bossNumber != 0) { continue; }
                if (_elapsed < 10) { continue; }

                // ボスを生成
                var bossEnemy = ecb.Instantiate(array.bossEnemyEntity);

                // SpawnPointSingletonDataが存在していた
                if (SystemAPI.HasSingleton<SpawnPointSingletonData>())
                {
                    // シングルトンデータの取得
                    var spawnPointSingleton = SystemAPI.GetSingleton<SpawnPointSingletonData>();

                    // 生成位置を取得
                    var spawnPoint = spawnPointSingleton.GetSpawnPoint
                        (
                            SpawnPointType.Center
                        );

                    // nullチェック。nullだったら原点に生成
                    spawnPoint = (spawnPoint == null) ? float3.zero : spawnPoint;

                    // LocalTransformを設定する
                    ecb.SetComponent(bossEnemy, new LocalTransform
                    {
                        Position = (float3)spawnPoint,
                        Rotation = quaternion.identity,
                        Scale = 1.0f
                    });
                }

                bossNumber--;
                continue;
            }

            var currentInfo = currentPattern.infos[currentInfoNumber];

            // 経過時間が敵生成時間未満だったらコンテニュー
            if (currentInfo.CreationDelay > _elapsed) { continue; }

            currentInfoNumber++;

            // EnemyNameに対応する敵Entityを取得
            var enemyEntity = GetEnemyEntity(ref state, currentInfo.MyEnemyName);

            // 敵EntityがNullだったらコンテニュー
            if (enemyEntity == Entity.Null) { continue; }

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
                    Scale = 1.0f
                });
            }
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
            Debug.Log($"enemyEntityData:{enemyEntityData.enemyName}, enemyName:{enemyName}");

            if (enemyEntityData.enemyName == enemyName)
            {
                return enemyEntityData.enemyEntity;
            }
        }

        // 見つからなかった
        return Entity.Null;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        _elapsed = 0.0f;
        currentPattern = new();
        currentInfoNumber = 0;
        bossNumber = 3;

        Debug.LogError("マジックナンバー");
    }
}

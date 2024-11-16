using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static EnemyHelper;

#if UNITY_EDITOR
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

    [Tooltip("EnemySpawnerSystemが有効か")]
    public bool isSelfEnable;

    public void OnUpdate(ref SystemState state)
    {
        // EntityQueryDesc を使って `Disabled` コンポーネントを除外
        var queryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(EntityEnemySpawnData), typeof(Disabled) }
        };

        // クエリ作成
        var query = state.EntityManager.CreateEntityQuery(queryDesc);

        // エンティティの数を取得
        int entityEnemySpawnCount = query.CalculateEntityCount();

        // クエリ作成
        var enabledEnemyQuery = state.EntityManager.CreateEntityQuery(typeof(EntityEnemySpawnData));

        // エンティティの数を取得
        int enabledEnemySpawnCount = enabledEnemyQuery.CalculateEntityCount();


        // まだEntityEnemySpawnDataが存在していない
        if (enabledEnemySpawnCount <= 0)
        {
            if (entityEnemySpawnCount <= 0)
            {
                return;
            }
        }



        _elapsed += SystemAPI.Time.DeltaTime;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Entityと一緒にDataを取得する
        foreach (var (entityEnemySpawnData, entity) in SystemAPI.Query<EntityEnemySpawnData>().WithEntityAccess())
        {
            // 経過時間が敵生成時間未満だったらコンテニュー
            if (entityEnemySpawnData.enemySpawnSettingInfo.CreationDelay > _elapsed) { continue; }

            var enemyEntity = entityEnemySpawnData.enemyEntity;

            // 敵EntityがNullだったらコンテニュー
            if (enemyEntity == Entity.Null) { continue; }

            // 敵を生成
            var enemy = ecb.Instantiate(enemyEntity);

            // 今回使用した敵生成情報を検索対象から外す
            ecb.AddComponent<Disabled>(entity);

            // ecbでDataをアタッチする
            ecb.AddComponent(enemy, new EnemyTag());

            // LocalTransformを設定する
            ecb.SetComponent(enemy, new LocalTransform
            {
                Position = entityEnemySpawnData.enemySpawnSettingInfo.SpawnPosition,
                Rotation = quaternion.identity,
                Scale = 1.0f
            });
        }

        if (enabledEnemySpawnCount <= 0)
        {
            // ゲームオーバー処理を開始する
            GameOver.Instance.OnGameOver();
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

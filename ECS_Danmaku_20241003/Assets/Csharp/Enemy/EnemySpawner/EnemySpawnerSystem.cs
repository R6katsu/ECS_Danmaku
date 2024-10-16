using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using static EnemyHelper;
using static MoveHelper;

/// <summary>
/// 敵の生成処理
/// </summary>
[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    private float _delta;

    public void OnUpdate(ref SystemState state)
    {
        Debug.Log("試験的なコードの為、後で直す");
        // 現状は敵を1秒置きに生成しているので生成方法を変える必要がある
        // 現状はDataを全て取得して生成しているので、敵の数が増えたら一回に敵の数生成することになってしまう
        _delta += SystemAPI.Time.DeltaTime;
        if (_delta < 1.0f) { return; }
        _delta = 0.0f;

        var entityManager = state.EntityManager;

        // CommandBufferを作成
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Entityと一緒にDataを取得する
        foreach (var (enemySpawner, entity) in SystemAPI.Query<EnemySpawnerData>().WithEntityAccess())
        {
            // 敵を生成
            var enemy = ecb.Instantiate(enemySpawner.enemyEntity);

            // ecbでDataをアタッチする
            ecb.AddComponent(enemy, new EnemyTag());
            ecb.AddComponent(enemy, new DirectionTravelData(enemySpawner.directionTravelType));

            // 線上からランダムな生成位置を求める
            float randomValue = UnityEngine.Random.value;
            var spawnPoint = Vector3.Lerp(enemySpawner.startPoint, enemySpawner.endPoint, randomValue);

            // LocalTransformを設定する
            ecb.SetComponent(enemy, new LocalTransform
            {
                Position = spawnPoint,
                Rotation = quaternion.identity,
                Scale = 1.0f
            });
        }

        // 変更を一括して適用
        ecb.Playback(entityManager);
        ecb.Dispose();
    }
}

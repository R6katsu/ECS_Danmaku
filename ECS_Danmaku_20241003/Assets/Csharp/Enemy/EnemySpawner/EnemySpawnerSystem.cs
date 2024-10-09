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

public partial struct EnemySpawnerSystem : ISystem
{
    private float _delta;

    public void OnUpdate(ref SystemState state)
    {
        _delta += SystemAPI.Time.DeltaTime;
        if (_delta < 1.0f) { return; }  // 遅延がないと必要なTagをアタッチする前に生成処理が呼び出されてしまう
        _delta = 0.0f;

        var entityManager = state.EntityManager;

        // CommandBufferを作成
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (enemySpawner, entity) in SystemAPI.Query<EnemySpawnerData>().WithEntityAccess())
        {
            var enemy = ecb.Instantiate(enemySpawner.enemyPrefab);

            ecb.AddComponent(enemy, new EnemyTag());
            ecb.AddComponent(enemy, new DirectionTravelData(enemySpawner.directionTravelType));

            // 線上からランダムな生成位置を求める
            float randomValue = UnityEngine.Random.value;
            var spawnPoint = Vector3.Lerp(enemySpawner.startPoint, enemySpawner.endPoint, randomValue);

            // Transformを代入
            ecb.SetComponent(enemy, new LocalTransform
            {
                Position = spawnPoint,
                Rotation = quaternion.identity,
                Scale = 1.0f    // 後で治す。ここはPositionだけ変更すればいい
            });
        }

        // 変更を一括して適用
        ecb.Playback(entityManager);
        ecb.Dispose();
    }
}

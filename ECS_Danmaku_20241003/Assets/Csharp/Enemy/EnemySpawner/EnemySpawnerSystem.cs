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
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// 敵の生成処理
/// </summary>
[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    private float _elapsed;

    [Tooltip("現在の生成要素番号")]
    private int _currentSpawnNumber;

    [Tooltip("EnemySpawnerSystemが有効か")]
    public bool isSelfEnable;

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<EnemyEntitySingletonData>()) { return; }

        Debug.Log("SceneTag追加");
        Debug.Log("複数回Setしてしまっている");
        Debug.Log("GetSingletonでいい。ループはしなくていい");



        // シーンに配置した奴でも駄目だった
        // Prefabとか、シーンに配置しているかとかは関係なさそう
        // var enemy = ecb.Instantiate(enemyEntity);
        // var enemy = entityManager.Instantiate(enemyEntity);
        // entity取得側に問題？LogではEntityが存在していたが、どっかで破棄されている？
        // インスタンスが違う？いや、取得自体はできている
        // 反映されていない？

        // Singleton.InstanceはSubSceneでは使えない
        // Entity化した敵はSingletonDataに保持させるべき
        Debug.LogError("Entity化した敵はSingletonDataに保持させるべき");

        // なんか!isSelfEnableができていない
        // 外部から変更した場合、変更した後に保存する必要があるのかも

        // EnemySpawnerSystemが有効ではなかった
        //if (!isSelfEnable) { return; }

        // 経過時間にフレーム秒数を加算代入
        //_elapsed += SystemAPI.Time.DeltaTime;

        //if (!SystemAPI.HasSingleton<EnemySpawnerSingletonData>()) { return; }
        //var enemySpawnerSingleton = SystemAPI.GetSingleton<EnemySpawnerSingletonData>();
        //Entity enemySpawnerEntity = SystemAPI.GetSingletonEntity<EnemySpawnerSingletonData>();

        //var enemySpawnInfos = enemySpawnerSingleton.enemySpawnInfos;
        //var currentEnemySpawnInfo = enemySpawnInfos[_currentSpawnNumber];

        //if (currentEnemySpawnInfo.CreationDelay > _elapsed) { return; }
        //_currentSpawnNumber++;

        //if (!SystemAPI.HasSingleton<EnemyEntitySingletonData>()) { return; }
        //var enemyEntitySingleton = SystemAPI.GetSingleton<EnemyEntitySingletonData>();
        //var enemyEntity = enemyEntitySingleton.GetEnemyEntity(currentEnemySpawnInfo.EnemyName);

        //var enemy = ecb.Instantiate(enemyEntity);

        //if (!SystemAPI.HasComponent<LocalTransform>(enemy)) { return; }

        //var localTfm = SystemAPI.GetComponent<LocalTransform>(enemy);

        //localTfm.Position = currentEnemySpawnInfo.SpawnPosition;

        /*
        Entity enemySpawnerEntity = SystemAPI.GetSingletonEntity<EnemyEntitySingletonData>();
        state.EntityManager.SetSharedComponentManaged(enemySpawnerEntity, new SceneTag { SceneEntity = enemySpawnerEntity });

        var enemyEntitySingleton = SystemAPI.GetSingleton<EnemyEntitySingletonData>();

        if (!SystemAPI.HasSingleton<EnemySpawnerSingletonData>()) { return; }
        var enemySpawnerSingleton = SystemAPI.GetSingleton<EnemySpawnerSingletonData>();

        var enemySpawnInfos = enemySpawnerSingleton.enemySpawnInfos;
        if (enemySpawnInfos.Length <= _currentSpawnNumber) { return; }

        var currentEnemySpawnInfo = enemySpawnInfos[_currentSpawnNumber];

        if (currentEnemySpawnInfo.CreationDelay > _elapsed) { return; }
        _currentSpawnNumber++;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Entityと一緒にDataを取得する
        foreach (var (enemySpawner, entity) in SystemAPI.Query<EnemySpawnerSingletonData>().WithEntityAccess())
        {
            var enemyEntity = enemyEntitySingleton.GetEnemyEntity(currentEnemySpawnInfo.EnemyName);

            // 敵を生成
            var enemy = ecb.Instantiate(enemyEntity);

            // ecbでDataをアタッチする
            ecb.AddComponent(enemy, new EnemyTag());
            ecb.AddComponent(enemy, new DirectionTravelData(enemySpawner.directionTravelType));

            // LocalTransformを設定する
            ecb.SetComponent(enemy, new LocalTransform
            {
                Position = currentEnemySpawnInfo.SpawnPosition,
                Rotation = quaternion.identity,
                Scale = 1.0f
            });
        }
        */
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        _elapsed += SystemAPI.Time.DeltaTime;

        // Entityと一緒にDataを取得する
        foreach (var (enemySpawner, entity) in SystemAPI.Query<EnemySpawnerSingletonData>().WithEntityAccess())
        {
            var enemySpawnInfos = enemySpawner.enemySpawnInfos;
            if (enemySpawnInfos.Length <= _currentSpawnNumber) { return; }

            var currentEnemySpawnInfo = enemySpawnInfos[_currentSpawnNumber];

            if (currentEnemySpawnInfo.CreationDelay > _elapsed) { return; }
            _currentSpawnNumber++;

            Debug.Log("EnemyEntitySingletonDataが原因っぽい");
            Debug.Log("SetSharedComponentManaged　SceneTagで大丈夫になったかとも思ったが駄目っぽい");
            Debug.Log("そもそも何故このSingletonDataだけエラーになるのかが分からない");
            Debug.Log("NoneをDynamicにしたり、ビルド時に固定をコメントアウトしても駄目だった");
            //if (!SystemAPI.HasSingleton<EnemyEntitySingletonData>()) { return; }
            //state.EntityManager.SetSharedComponentManaged(entity, new SceneTag { SceneEntity = entity });
            //var enemyEntitySingleton = SystemAPI.GetSingleton<EnemyEntitySingletonData>();
            //var enemyEntity = enemyEntitySingleton.GetEnemyEntity(currentEnemySpawnInfo.EnemyName);

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

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

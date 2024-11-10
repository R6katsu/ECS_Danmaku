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
/// �G�̐�������
/// </summary>
[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    private float _elapsed;

    [Tooltip("���݂̐����v�f�ԍ�")]
    private int _currentSpawnNumber;

    [Tooltip("EnemySpawnerSystem���L����")]
    public bool isSelfEnable;

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<EnemyEntitySingletonData>()) { return; }

        Debug.Log("SceneTag�ǉ�");
        Debug.Log("������Set���Ă��܂��Ă���");
        Debug.Log("GetSingleton�ł����B���[�v�͂��Ȃ��Ă���");



        // �V�[���ɔz�u�����z�ł��ʖڂ�����
        // Prefab�Ƃ��A�V�[���ɔz�u���Ă��邩�Ƃ��͊֌W�Ȃ�����
        // var enemy = ecb.Instantiate(enemyEntity);
        // var enemy = entityManager.Instantiate(enemyEntity);
        // entity�擾���ɖ��HLog�ł�Entity�����݂��Ă������A�ǂ����Ŕj������Ă���H
        // �C���X�^���X���Ⴄ�H����A�擾���̂͂ł��Ă���
        // ���f����Ă��Ȃ��H

        // Singleton.Instance��SubScene�ł͎g���Ȃ�
        // Entity�������G��SingletonData�ɕێ�������ׂ�
        Debug.LogError("Entity�������G��SingletonData�ɕێ�������ׂ�");

        // �Ȃ�!isSelfEnable���ł��Ă��Ȃ�
        // �O������ύX�����ꍇ�A�ύX������ɕۑ�����K�v������̂���

        // EnemySpawnerSystem���L���ł͂Ȃ�����
        //if (!isSelfEnable) { return; }

        // �o�ߎ��ԂɃt���[���b�������Z���
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

        // Entity�ƈꏏ��Data���擾����
        foreach (var (enemySpawner, entity) in SystemAPI.Query<EnemySpawnerSingletonData>().WithEntityAccess())
        {
            var enemyEntity = enemyEntitySingleton.GetEnemyEntity(currentEnemySpawnInfo.EnemyName);

            // �G�𐶐�
            var enemy = ecb.Instantiate(enemyEntity);

            // ecb��Data���A�^�b�`����
            ecb.AddComponent(enemy, new EnemyTag());
            ecb.AddComponent(enemy, new DirectionTravelData(enemySpawner.directionTravelType));

            // LocalTransform��ݒ肷��
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

        // Entity�ƈꏏ��Data���擾����
        foreach (var (enemySpawner, entity) in SystemAPI.Query<EnemySpawnerSingletonData>().WithEntityAccess())
        {
            var enemySpawnInfos = enemySpawner.enemySpawnInfos;
            if (enemySpawnInfos.Length <= _currentSpawnNumber) { return; }

            var currentEnemySpawnInfo = enemySpawnInfos[_currentSpawnNumber];

            if (currentEnemySpawnInfo.CreationDelay > _elapsed) { return; }
            _currentSpawnNumber++;

            Debug.Log("EnemyEntitySingletonData���������ۂ�");
            Debug.Log("SetSharedComponentManaged�@SceneTag�ő��v�ɂȂ������Ƃ��v�������ʖڂ��ۂ�");
            Debug.Log("�����������̂���SingletonData�����G���[�ɂȂ�̂���������Ȃ�");
            Debug.Log("None��Dynamic�ɂ�����A�r���h���ɌŒ���R�����g�A�E�g���Ă��ʖڂ�����");
            //if (!SystemAPI.HasSingleton<EnemyEntitySingletonData>()) { return; }
            //state.EntityManager.SetSharedComponentManaged(entity, new SceneTag { SceneEntity = entity });
            //var enemyEntitySingleton = SystemAPI.GetSingleton<EnemyEntitySingletonData>();
            //var enemyEntity = enemyEntitySingleton.GetEnemyEntity(currentEnemySpawnInfo.EnemyName);

            // �G�𐶐�
            var enemy = ecb.Instantiate(enemySpawner.enemyEntity);

            // ecb��Data���A�^�b�`����
            ecb.AddComponent(enemy, new EnemyTag());
            ecb.AddComponent(enemy, new DirectionTravelData(enemySpawner.directionTravelType));

            // ���ォ�烉���_���Ȑ����ʒu�����߂�
            float randomValue = UnityEngine.Random.value;
            var spawnPoint = Vector3.Lerp(enemySpawner.startPoint, enemySpawner.endPoint, randomValue);

            // LocalTransform��ݒ肷��
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

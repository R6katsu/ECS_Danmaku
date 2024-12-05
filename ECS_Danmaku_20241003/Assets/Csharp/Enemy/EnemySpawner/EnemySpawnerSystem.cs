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
/// �G�̐�������
/// </summary>
[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    private float _elapsed;

    EnemySpawnPattern currentPattern;
    int currentInfoNumber;

    int bossNumber;

    [Tooltip("EnemySpawnerSystem���L����")]
    public bool isSelfEnable;

    public void OnCreate(ref SystemState state)
    {
        Initialize();
    }

    public void OnUpdate(ref SystemState state)
    {
        // EnemySpawnPatternArraySingletonData�����݂��Ȃ�����
        if (!SystemAPI.HasSingleton<EnemySpawnPatternArraySingletonData>())
        {
            // ������
            Initialize();
        }

        _elapsed += SystemAPI.Time.DeltaTime;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Entity�ƈꏏ��Data���擾����
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

            // 0�ȉ��ɂȂ���
            if (bossNumber <= 0)
            {
                if (bossNumber != 0) { continue; }
                if (_elapsed < 10) { continue; }

                // �{�X�𐶐�
                var bossEnemy = ecb.Instantiate(array.bossEnemyEntity);

                // SpawnPointSingletonData�����݂��Ă���
                if (SystemAPI.HasSingleton<SpawnPointSingletonData>())
                {
                    // �V���O���g���f�[�^�̎擾
                    var spawnPointSingleton = SystemAPI.GetSingleton<SpawnPointSingletonData>();

                    // �����ʒu���擾
                    var spawnPoint = spawnPointSingleton.GetSpawnPoint
                        (
                            SpawnPointType.Center
                        );

                    // null�`�F�b�N�Bnull�������猴�_�ɐ���
                    spawnPoint = (spawnPoint == null) ? float3.zero : spawnPoint;

                    // LocalTransform��ݒ肷��
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

            // �o�ߎ��Ԃ��G�������Ԗ�����������R���e�j���[
            if (currentInfo.CreationDelay > _elapsed) { continue; }

            currentInfoNumber++;

            // EnemyName�ɑΉ�����GEntity���擾
            var enemyEntity = GetEnemyEntity(ref state, currentInfo.MyEnemyName);

            // �GEntity��Null��������R���e�j���[
            if (enemyEntity == Entity.Null) { continue; }

            // �G�𐶐�
            var enemy = ecb.Instantiate(enemyEntity);

            // SpawnPointSingletonData�����݂��Ă���
            if (SystemAPI.HasSingleton<SpawnPointSingletonData>())
            {
                // �V���O���g���f�[�^�̎擾
                var spawnPointSingleton = SystemAPI.GetSingleton<SpawnPointSingletonData>();

                // �����ʒu���擾
                var spawnPoint = spawnPointSingleton.GetSpawnPoint
                    (
                        currentInfo.SpawnPointType
                    );

                // null�`�F�b�N�Bnull�������猴�_�ɐ���
                spawnPoint = (spawnPoint == null) ? float3.zero : spawnPoint;

                // LocalTransform��ݒ肷��
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
    /// EnemyName�ɑΉ�����GEntity���擾����
    /// </summary>
    /// <param name="enemyName">�GEntity�̖���</param>
    /// <returns>EnemyName�ɑΉ�����GEntity</returns>
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

        // ������Ȃ�����
        return Entity.Null;
    }

    /// <summary>
    /// ������
    /// </summary>
    private void Initialize()
    {
        _elapsed = 0.0f;
        currentPattern = new();
        currentInfoNumber = 0;
        bossNumber = 3;

        Debug.LogError("�}�W�b�N�i���o�[");
    }
}

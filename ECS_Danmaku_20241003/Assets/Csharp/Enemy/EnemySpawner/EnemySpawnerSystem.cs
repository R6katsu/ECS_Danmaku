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
/// �G�̐�������
/// </summary>
[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    private float _elapsed;

    [Tooltip("EnemySpawnerSystem���L����")]
    public bool isSelfEnable;

    public void OnUpdate(ref SystemState state)
    {
        // EntityQueryDesc ���g���� `Disabled` �R���|�[�l���g�����O
        var queryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(EntityEnemySpawnData), typeof(Disabled) }
        };

        // �N�G���쐬
        var query = state.EntityManager.CreateEntityQuery(queryDesc);

        // �G���e�B�e�B�̐����擾
        int entityEnemySpawnCount = query.CalculateEntityCount();

        // �N�G���쐬
        var enabledEnemyQuery = state.EntityManager.CreateEntityQuery(typeof(EntityEnemySpawnData));

        // �G���e�B�e�B�̐����擾
        int enabledEnemySpawnCount = enabledEnemyQuery.CalculateEntityCount();


        // �܂�EntityEnemySpawnData�����݂��Ă��Ȃ�
        if (enabledEnemySpawnCount <= 0)
        {
            if (entityEnemySpawnCount <= 0)
            {
                return;
            }
        }



        _elapsed += SystemAPI.Time.DeltaTime;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Entity�ƈꏏ��Data���擾����
        foreach (var (entityEnemySpawnData, entity) in SystemAPI.Query<EntityEnemySpawnData>().WithEntityAccess())
        {
            // �o�ߎ��Ԃ��G�������Ԗ�����������R���e�j���[
            if (entityEnemySpawnData.enemySpawnSettingInfo.CreationDelay > _elapsed) { continue; }

            var enemyEntity = entityEnemySpawnData.enemyEntity;

            // �GEntity��Null��������R���e�j���[
            if (enemyEntity == Entity.Null) { continue; }

            // �G�𐶐�
            var enemy = ecb.Instantiate(enemyEntity);

            // ����g�p�����G�������������Ώۂ���O��
            ecb.AddComponent<Disabled>(entity);

            // ecb��Data���A�^�b�`����
            ecb.AddComponent(enemy, new EnemyTag());

            // LocalTransform��ݒ肷��
            ecb.SetComponent(enemy, new LocalTransform
            {
                Position = entityEnemySpawnData.enemySpawnSettingInfo.SpawnPosition,
                Rotation = quaternion.identity,
                Scale = 1.0f
            });
        }

        if (enabledEnemySpawnCount <= 0)
        {
            // �Q�[���I�[�o�[�������J�n����
            GameOver.Instance.OnGameOver();
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

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
        if (_delta < 1.0f) { return; }  // �x�����Ȃ��ƕK�v��Tag���A�^�b�`����O�ɐ����������Ăяo����Ă��܂�
        _delta = 0.0f;

        var entityManager = state.EntityManager;

        // CommandBuffer���쐬
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (enemySpawner, entity) in SystemAPI.Query<EnemySpawnerData>().WithEntityAccess())
        {
            var enemy = ecb.Instantiate(enemySpawner.enemyPrefab);

            ecb.AddComponent(enemy, new EnemyTag());
            ecb.AddComponent(enemy, new DirectionTravelData(enemySpawner.directionTravelType));

            // ���ォ�烉���_���Ȑ����ʒu�����߂�
            float randomValue = UnityEngine.Random.value;
            var spawnPoint = Vector3.Lerp(enemySpawner.startPoint, enemySpawner.endPoint, randomValue);

            // Transform����
            ecb.SetComponent(enemy, new LocalTransform
            {
                Position = spawnPoint,
                Rotation = quaternion.identity,
                Scale = 1.0f    // ��Ŏ����B������Position�����ύX����΂���
            });
        }

        // �ύX���ꊇ���ēK�p
        ecb.Playback(entityManager);
        ecb.Dispose();
    }
}

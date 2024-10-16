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
/// �G�̐�������
/// </summary>
[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    private float _delta;

    public void OnUpdate(ref SystemState state)
    {
        Debug.Log("�����I�ȃR�[�h�ׁ̈A��Œ���");
        // ����͓G��1�b�u���ɐ������Ă���̂Ő������@��ς���K�v������
        // �����Data��S�Ď擾���Đ������Ă���̂ŁA�G�̐�������������ɓG�̐��������邱�ƂɂȂ��Ă��܂�
        _delta += SystemAPI.Time.DeltaTime;
        if (_delta < 1.0f) { return; }
        _delta = 0.0f;

        var entityManager = state.EntityManager;

        // CommandBuffer���쐬
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Entity�ƈꏏ��Data���擾����
        foreach (var (enemySpawner, entity) in SystemAPI.Query<EnemySpawnerData>().WithEntityAccess())
        {
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

        // �ύX���ꊇ���ēK�p
        ecb.Playback(entityManager);
        ecb.Dispose();
    }
}

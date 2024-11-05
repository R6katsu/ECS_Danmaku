using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// �r���{�[�h
/// </summary>
public struct BillboardTag : IComponentData { }

/// <summary>
/// �r���{�[�h�̏���
/// </summary>
[BurstCompile]
public partial struct BillboardSystem : ISystem
{
    // Quad�ł͔��Ό����ɂȂ��Ă��܂�
    // �����Ă�������ɐi�ޏ����Ƃ̑������ň�
    // �z�[�~���O�e�ł������Ă�������ɐi�܂�����
    // ���̏ꍇ�A�����ׂ��̓r���{�[�h���Ƃ������ƂɂȂ�
    // �ړ��������A�^�b�`���Ă���I�u�W�F�N�g�ƌ����ڂ̃I�u�W�F�N�g��ʕ��ɂ���

    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // Camera��null�łȂ���Έʒu���擾����
        float3 cameraPosition = (Camera.main != null) ? Camera.main.transform.position : float3.zero;

        foreach (var (billboard, localTfm) in
                 SystemAPI.Query<RefRO<BillboardTag>,
                 RefRW<LocalTransform>>())
        {
            float3 direction = cameraPosition - localTfm.ValueRO.Position;

            // Y���̉�]�𖳎�
            direction.y = 0;

            if (math.length(direction) > 0.01f) // �[���łȂ��ꍇ
            {
                Quaternion rotation = Quaternion.LookRotation(math.normalize(direction));
                localTfm.ValueRW.Rotation = rotation;
            }
        }
    }
}

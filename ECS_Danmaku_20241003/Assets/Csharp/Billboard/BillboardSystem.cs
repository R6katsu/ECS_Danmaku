using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
using System.Diagnostics;
using TMPro;
#endif

/// <summary>
/// �r���{�[�h�̏���
/// </summary>
[BurstCompile]
public partial struct BillboardSystem : ISystem
{
    // �v�������ʓ|������������
    // ���������e����Particle�AVFX���g�����ꍇ�Ǝg��Ȃ��ꍇ���Ƃǂ��炪�����̂�
    // ����AECS���g���ꍇ��VFX���g��Ȃ������ǂ��BVFX��ECS�ɕϊ��ł��Ȃ��ׁA������VFX�ɂȂ�

    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // Camera��null�łȂ���Έʒu���擾����
        float3 cameraPosition = (Camera.main != null) ? Camera.main.transform.position : float3.zero;

        foreach (var (billboard, parent, localTfm) in
                 SystemAPI.Query<RefRO<BillboardTag>,
                 RefRO<Parent>,
                 RefRW<LocalTransform>>())
        {
            /*
            // �e����ʒu���擾���A�J�����̕��������߂�B�e���Ȃ���Ύ��g�̈ʒu���g�p
            float3 direction = (entityManager.HasComponent<LocalTransform>(parent.ValueRO.Value)) ?
                  cameraPosition - entityManager.GetComponentData<LocalTransform>(parent.ValueRO.Value).Position
                : cameraPosition - localTfm.ValueRO.Position;

            // Y���̉�]�𖳎�
            direction.y = 0;

            if (math.length(direction) > 0.01f) // �[���łȂ��ꍇ
            {
                Quaternion rotation = Quaternion.LookRotation(math.normalize(direction));
                localTfm.ValueRW.Rotation = rotation;
            }
            */

            //HAIO(billboard, parent, localTfm, ref entityManager, cameraPosition);
        }
    }

    /*
    public void HAIO(RefRO<BillboardTag> billboard, RefRO<Parent> parent, RefRW<LocalTransform> localTfm, ref EntityManager entityManager, float3 cameraPosition)
    {
        // �e����ʒu���擾���A�J�����̕��������߂�B�e���Ȃ���Ύ��g�̈ʒu���g�p
        //float3 direction = (entityManager.HasComponent<LocalTransform>(parent.ValueRO.Value)) ?
        //      cameraPosition - entityManager.GetComponentData<LocalTransform>(parent.ValueRO.Value).Position
        //    : cameraPosition - localTfm.ValueRO.Position;

        float3 direction = cameraPosition - entityManager.GetComponentData<LocalTransform>(parent.ValueRO.Value).Position;

        // Y���̉�]�𖳎�
        //direction.y = 0;

        //direction.x *= -1.0f;
        //direction.y *= -1.0f;
        //direction.z *= -1.0f;

        // �[���łȂ��ꍇ
        if (math.length(direction) > 0.01f)
        {
            //quaternion rotation = quaternion.LookRotationSafe(math.normalize(direction), math.up());
            // �J���������������N�H�[�^�j�I�����쐬
            quaternion targetRotation = quaternion.LookRotationSafe(math.normalize(direction), math.up());

            // ��]��K�p
            localTfm.ValueRW.Rotation = targetRotation;

            Debug.Log($"��]:{targetRotation}");
        }
    }
    */
}

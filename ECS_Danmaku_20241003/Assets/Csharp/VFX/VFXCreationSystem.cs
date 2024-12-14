using Unity.Entities;
using Unity.Transforms;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using static VFXCreationBridge;
using static UnityEngine.EventSystems.EventTrigger;
using Unity.VisualScripting.FullSerializer;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// VFX�𐶐�����B<br/>
/// ���C���X���b�h�Ŋ֐����Ăяo���ׂ�System�B
/// </summary>
public partial struct VFXCreationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // VFX�𐶐�����
        foreach (var (vfxCreation, localTfm) in
                 SystemAPI.Query<RefRO<VFXCreationData>,
                                 RefRW<LocalTransform>>())
        {
            // �����\�t���O�������Ă��Ȃ���΃R���e�j���[
            if (!vfxCreation.ValueRO.IsCreatable) { continue; }

            VFXCreationBridge.Instance.VFXCreation
           (
               vfxCreation.ValueRO.visualEffectName,
               vfxCreation.ValueRO.Position,
               vfxCreation.ValueRO.size
           );

            // �����𖳌��ɂ���
            vfxCreation.ValueRO.DisableCreation();
        }
    }
}

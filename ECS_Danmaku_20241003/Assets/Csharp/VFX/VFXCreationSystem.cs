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

// リファクタリング済み

/// <summary>
/// VFXを生成する。<br/>
/// メインスレッドで関数を呼び出す為のSystem。
/// </summary>
public partial struct VFXCreationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // VFXを生成する
        foreach (var (vfxCreation, localTfm) in
                 SystemAPI.Query<RefRO<VFXCreationData>,
                                 RefRW<LocalTransform>>())
        {
            // 生成可能フラグが立っていなければコンテニュー
            if (!vfxCreation.ValueRO.IsCreatable) { continue; }

            VFXCreationBridge.Instance.VFXCreation
           (
               vfxCreation.ValueRO.visualEffectName,
               vfxCreation.ValueRO.Position,
               vfxCreation.ValueRO.size
           );

            // 生成を無効にする
            vfxCreation.ValueRO.DisableCreation();
        }
    }
}

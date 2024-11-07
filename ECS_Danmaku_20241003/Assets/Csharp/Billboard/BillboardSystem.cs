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
/// ビルボードの処理
/// </summary>
[BurstCompile]
public partial struct BillboardSystem : ISystem
{
    // 思ったより面倒だったから後回し
    // そもそも弾幕はParticle、VFXを使った場合と使わない場合だとどちらがいいのか
    // いや、ECSを使う場合はVFXを使わない方が良い。VFXはECSに変換できない為、ただのVFXになる

    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // Cameraがnullでなければ位置を取得する
        float3 cameraPosition = (Camera.main != null) ? Camera.main.transform.position : float3.zero;

        foreach (var (billboard, parent, localTfm) in
                 SystemAPI.Query<RefRO<BillboardTag>,
                 RefRO<Parent>,
                 RefRW<LocalTransform>>())
        {
            /*
            // 親から位置を取得し、カメラの方向を求める。親がなければ自身の位置を使用
            float3 direction = (entityManager.HasComponent<LocalTransform>(parent.ValueRO.Value)) ?
                  cameraPosition - entityManager.GetComponentData<LocalTransform>(parent.ValueRO.Value).Position
                : cameraPosition - localTfm.ValueRO.Position;

            // Y軸の回転を無視
            direction.y = 0;

            if (math.length(direction) > 0.01f) // ゼロでない場合
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
        // 親から位置を取得し、カメラの方向を求める。親がなければ自身の位置を使用
        //float3 direction = (entityManager.HasComponent<LocalTransform>(parent.ValueRO.Value)) ?
        //      cameraPosition - entityManager.GetComponentData<LocalTransform>(parent.ValueRO.Value).Position
        //    : cameraPosition - localTfm.ValueRO.Position;

        float3 direction = cameraPosition - entityManager.GetComponentData<LocalTransform>(parent.ValueRO.Value).Position;

        // Y軸の回転を無視
        //direction.y = 0;

        //direction.x *= -1.0f;
        //direction.y *= -1.0f;
        //direction.z *= -1.0f;

        // ゼロでない場合
        if (math.length(direction) > 0.01f)
        {
            //quaternion rotation = quaternion.LookRotationSafe(math.normalize(direction), math.up());
            // カメラ方向を向くクォータニオンを作成
            quaternion targetRotation = quaternion.LookRotationSafe(math.normalize(direction), math.up());

            // 回転を適用
            localTfm.ValueRW.Rotation = targetRotation;

            Debug.Log($"回転:{targetRotation}");
        }
    }
    */
}

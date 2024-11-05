using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// ビルボード
/// </summary>
public struct BillboardTag : IComponentData { }

/// <summary>
/// ビルボードの処理
/// </summary>
[BurstCompile]
public partial struct BillboardSystem : ISystem
{
    // Quadでは反対向きになってしまう
    // 向いている方向に進む処理との相性が最悪
    // ホーミング弾でも向いている方向に進ませたい
    // この場合、直すべきはビルボード側ということになる
    // 移動処理をアタッチしているオブジェクトと見た目のオブジェクトを別物にする

    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // Cameraがnullでなければ位置を取得する
        float3 cameraPosition = (Camera.main != null) ? Camera.main.transform.position : float3.zero;

        foreach (var (billboard, localTfm) in
                 SystemAPI.Query<RefRO<BillboardTag>,
                 RefRW<LocalTransform>>())
        {
            float3 direction = cameraPosition - localTfm.ValueRO.Position;

            // Y軸の回転を無視
            direction.y = 0;

            if (math.length(direction) > 0.01f) // ゼロでない場合
            {
                Quaternion rotation = Quaternion.LookRotation(math.normalize(direction));
                localTfm.ValueRW.Rotation = rotation;
            }
        }
    }
}

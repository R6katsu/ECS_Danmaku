using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// 弾の処理
/// </summary>
public partial struct BulletSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;
        var delta = SystemAPI.Time.DeltaTime;

        foreach (var (bullet, destroyable) in
                 SystemAPI.Query<RefRW<BulletData>,
                                 RefRW<DestroyableData>>())
        {
            bullet.ValueRW.elapsed += delta;

            // 経過時間が寿命未満だった
            if (bullet.ValueRW.elapsed < bullet.ValueRW.lifeTime) { continue; }

            // 寿命を迎えたので削除フラグを立てる
            destroyable.ValueRW.isKilled = true;
        }
    }
}

using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
#endif

// リファクタリング済み

/// <summary>
/// 弾を消す職能
/// </summary>
public struct BulletEraseTag : IComponentData { }

/// <summary>
/// 弾を消す職能の設定
/// </summary>
public class BulletEraseAuthoring : MonoBehaviour
{
    public class Baker : Baker<BulletEraseAuthoring>
    {
        public override void Bake(BulletEraseAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BulletEraseTag());
        }
    }
}

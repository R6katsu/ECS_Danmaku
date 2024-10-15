using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static BulletHelper;
using static EnemyHelper;
using static HealthHelper;

/// <summary>
/// 弾の設定
/// </summary>
public class BulletAuthoring : MonoBehaviour
{
    [SerializeField, Min(0.0f), Header("ダメージ量")]
    private float _damageAmount = 0.0f;

    /// <summary>
    /// ダメージ量
    /// </summary>
    public float DamageAmount => _damageAmount;

    public class Baker : Baker<BulletAuthoring>
    {
        public override unsafe void Bake(BulletAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new BulletTag());
            AddComponent(entity, new BulletIDealDamageData(src.DamageAmount));
        }
    }
}

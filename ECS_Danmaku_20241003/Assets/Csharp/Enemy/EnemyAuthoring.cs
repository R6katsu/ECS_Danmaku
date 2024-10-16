using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static EnemyHelper;
using static PlayerHelper;

/// <summary>
/// 敵の設定
/// </summary>
public class EnemyAuthoring : MonoBehaviour
{
    [SerializeField, Min(0.0f), Header("最大体力")]
    private float _maxHP = 0.0f;

    [SerializeField, Min(0.0f), Header("無敵時間の長さ")]
    private float _isInvincibleTime = 0.0f;

    /// <summary>
    /// 無敵時間の長さ
    /// </summary>
    public float IsInvincibleTime => _isInvincibleTime;

    /// <summary>
    /// 最大体力
    /// </summary>
    public float MaxHP => _maxHP;

    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Dataをアタッチ
            AddComponent(entity, new EnemyTag());
            AddComponent(entity, new DestroyableData());
            AddComponent(entity, new EnemyHealthPointData(src.MaxHP, src.IsInvincibleTime));
        }
    }
}

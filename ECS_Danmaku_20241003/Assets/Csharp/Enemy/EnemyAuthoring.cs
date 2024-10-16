using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static EnemyHelper;
using static PlayerHelper;

/// <summary>
/// “G‚Ìİ’è
/// </summary>
public class EnemyAuthoring : MonoBehaviour
{
    [SerializeField, Min(0.0f), Header("Å‘å‘Ì—Í")]
    private float _maxHP = 0.0f;

    [SerializeField, Min(0.0f), Header("–³“GŠÔ‚Ì’·‚³")]
    private float _isInvincibleTime = 0.0f;

    /// <summary>
    /// –³“GŠÔ‚Ì’·‚³
    /// </summary>
    public float IsInvincibleTime => _isInvincibleTime;

    /// <summary>
    /// Å‘å‘Ì—Í
    /// </summary>
    public float MaxHP => _maxHP;

    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Data‚ğƒAƒ^ƒbƒ`
            AddComponent(entity, new EnemyTag());
            AddComponent(entity, new DestroyableData());
            AddComponent(entity, new EnemyHealthPointData(src.MaxHP, src.IsInvincibleTime));
        }
    }
}

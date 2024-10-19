using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static EnemyHelper;
using static EntityCampsHelper;
using static EntityCategoryHelper;
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

    [SerializeField, Header("w‰c‚Ìí—Ş")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("Entity‚ÌƒJƒeƒSƒŠ")]
    private EntityCategory _entityCategory = 0;

    /// <summary>
    /// –³“GŠÔ‚Ì’·‚³
    /// </summary>
    public float IsInvincibleTime => _isInvincibleTime;

    /// <summary>
    /// Å‘å‘Ì—Í
    /// </summary>
    public float MaxHP => _maxHP;

    /// <summary>
    /// w‰c‚Ìí—Ş
    /// </summary>
    public EntityCampsType CampsType => _campsType;

    /// <summary>
    /// Entity‚ÌƒJƒeƒSƒŠ
    /// </summary>
    public EntityCategory EntityCategory => _entityCategory;

    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Data‚ğƒAƒ^ƒbƒ`
            AddComponent(entity, new EnemyTag());
            AddComponent(entity, new DestroyableData());
            AddComponent(entity, new EnemyHealthPointData(src.MaxHP, src.IsInvincibleTime));

            // w‰c‚ÆƒJƒeƒSƒŠ‚ÌTag‚ğƒAƒ^ƒbƒ`
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src.CampsType));
            AddComponent(entity, EntityCategoryHelper.GetCampsTagType(src.EntityCategory));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static BulletHelper;
using static EntityCampsHelper;
using static EnemyHelper;
using static HealthHelper;
using static EntityCategoryHelper;

/// <summary>
/// ’e‚Ìİ’è
/// </summary>
public class BulletAuthoring : MonoBehaviour
{
    [SerializeField, Min(0.0f), Header("ƒ_ƒ[ƒW—Ê")]
    private float _damageAmount = 0.0f;

    [SerializeField, Header("w‰c‚Ìí—Ş")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("Entity‚ÌƒJƒeƒSƒŠ")]
    private EntityCategory _entityCategory = 0;

    /// <summary>
    /// ƒ_ƒ[ƒW—Ê
    /// </summary>
    public float DamageAmount => _damageAmount;

    /// <summary>
    /// w‰c‚Ìí—Ş
    /// </summary>
    public EntityCampsType CampsType => _campsType;

    /// <summary>
    /// Entity‚ÌƒJƒeƒSƒŠ
    /// </summary>
    public EntityCategory EntityCategory => _entityCategory;

    public class Baker : Baker<BulletAuthoring>
    {
        public override unsafe void Bake(BulletAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new BulletTag());
            AddComponent(entity, new BulletIDealDamageData(src.DamageAmount, src.CampsType));

            // w‰c‚ÆƒJƒeƒSƒŠ‚ÌTag‚ğƒAƒ^ƒbƒ`
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src.CampsType));
            AddComponent(entity, EntityCategoryHelper.GetCampsTagType(src.EntityCategory));
        }
    }
}

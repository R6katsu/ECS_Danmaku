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
/// eΜέθ
/// </summary>
public class BulletAuthoring : MonoBehaviour
{
    [SerializeField, Min(0.0f), Header("_[WΚ")]
    private float _damageAmount = 0.0f;

    [SerializeField, Header("wcΜνή")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("EntityΜJeS")]
    private EntityCategory _entityCategory = 0;

    /// <summary>
    /// _[WΚ
    /// </summary>
    public float DamageAmount => _damageAmount;

    /// <summary>
    /// wcΜνή
    /// </summary>
    public EntityCampsType CampsType => _campsType;

    /// <summary>
    /// EntityΜJeS
    /// </summary>
    public EntityCategory EntityCategory => _entityCategory;

    public class Baker : Baker<BulletAuthoring>
    {
        public override unsafe void Bake(BulletAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new BulletTag());
            AddComponent(entity, new BulletIDealDamageData(src.DamageAmount, src.CampsType));

            // wcΖJeSΜTagπA^b`
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src.CampsType));
            AddComponent(entity, EntityCategoryHelper.GetCampsTagType(src.EntityCategory));
        }
    }
}

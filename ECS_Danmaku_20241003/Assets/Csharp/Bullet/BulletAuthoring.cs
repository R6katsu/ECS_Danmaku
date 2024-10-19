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
/// �e�̐ݒ�
/// </summary>
public class BulletAuthoring : MonoBehaviour
{
    [SerializeField, Min(0.0f), Header("�_���[�W��")]
    private float _damageAmount = 0.0f;

    [SerializeField, Header("�w�c�̎��")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("Entity�̃J�e�S��")]
    private EntityCategory _entityCategory = 0;

    /// <summary>
    /// �_���[�W��
    /// </summary>
    public float DamageAmount => _damageAmount;

    /// <summary>
    /// �w�c�̎��
    /// </summary>
    public EntityCampsType CampsType => _campsType;

    /// <summary>
    /// Entity�̃J�e�S��
    /// </summary>
    public EntityCategory EntityCategory => _entityCategory;

    public class Baker : Baker<BulletAuthoring>
    {
        public override unsafe void Bake(BulletAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new BulletTag());
            AddComponent(entity, new BulletIDealDamageData(src.DamageAmount, src.CampsType));

            // �w�c�ƃJ�e�S����Tag���A�^�b�`
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src.CampsType));
            AddComponent(entity, EntityCategoryHelper.GetCampsTagType(src.EntityCategory));
        }
    }
}

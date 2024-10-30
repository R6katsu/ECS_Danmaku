using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static EnemyHelper;
using static EntityCampsHelper;
using static EntityCategoryHelper;
using static HealthPointDatas;

/// <summary>
/// �G�̐ݒ�
/// </summary>
public class EnemyAuthoring : MonoBehaviour
{
    [SerializeField, Min(0.0f), Header("�ő�̗�")]
    private float _maxHP = 0.0f;

    [SerializeField, Min(0.0f), Header("���G���Ԃ̒���")]
    private float _isInvincibleTime = 0.0f;

    [SerializeField, Header("�w�c�̎��")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("Entity�̃J�e�S��")]
    private EntityCategory _entityCategory = 0;

    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Data���A�^�b�`
            AddComponent(entity, new EnemyTag());
            AddComponent(entity, new DestroyableData());
            AddComponent(entity, new EnemyHealthPointData(src._maxHP, src._isInvincibleTime));

            // �w�c�ƃJ�e�S����Tag���A�^�b�`
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src._campsType));
            AddComponent(entity, EntityCategoryHelper.GetCategoryTagType(src._entityCategory));
        }
    }
}

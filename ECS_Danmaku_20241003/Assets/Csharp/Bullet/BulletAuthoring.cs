using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static BulletHelper;
using static EntityCampsHelper;
using static EntityCategoryHelper;

#if UNITY_EDITOR
#endif

/// <summary>
/// �e�̏��
/// </summary>
public struct BulletData : IComponentData
{
    [Tooltip("����")]
    public readonly float lifeTime;

    [Tooltip("�o�ߎ���")]
    public float elapsed;

    /// <summary>
    /// �e�̏��
    /// </summary>
    /// <param name="lifeTime">����</param>
    public BulletData(float lifeTime)
    {
        this.lifeTime = lifeTime;
        elapsed = 0.0f;
    }
}

/// <summary>
/// �e�̐ݒ�
/// </summary>
public class BulletAuthoring : MonoBehaviour
{
    [Tooltip("�������Ɋђʂ��鉺��")]
    private const int UNLIMITED_PIERCE_MINIMUM = -1;

    [SerializeField, Min(0.0f), Header("�_���[�W��")]
    private float _damageAmount = 0.0f;

    [SerializeField, Min(0.0f), Header("����")]
    private float _lifeTime = 0.0f;

    [SerializeField, Min(UNLIMITED_PIERCE_MINIMUM), Header("�c��ђʉ񐔁i���� = �����j")]
    private int _remainingPierceCount = 0;

    [SerializeField, Header("�w�c�̎��")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("Entity�̃J�e�S��")]
    private EntityCategory _entityCategory = 0;

    public class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // �c��ђʉ񐔂������ł͂Ȃ�����
            if (src._remainingPierceCount != BulletAuthoring.UNLIMITED_PIERCE_MINIMUM)
            {
                // �c��ђʉ񐔂�Data���A�^�b�`
                AddComponent(entity, new RemainingPierceCountData(src._remainingPierceCount));
            }

            AddComponent(entity, new BulletTag());
            AddComponent(entity, new DestroyableData());
            AddComponent(entity, new BulletData(src._lifeTime));
            AddComponent(entity, new BulletIDealDamageData(src._damageAmount, src._campsType));

            // �w�c�ƃJ�e�S����Tag���A�^�b�`
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src._campsType));
            AddComponent(entity, EntityCategoryHelper.GetCategoryTagType(src._entityCategory));
        }
    }
}

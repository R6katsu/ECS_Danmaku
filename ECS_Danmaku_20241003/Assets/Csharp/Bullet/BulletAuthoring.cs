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
    // �Ȃ��A����BulletAuthoring�������ǂݍ��܂�Ȃ��Ȃ�H
    // �������R�s�y���đ���Bake�ɓ\��t�����Ƃ���@�\����
    // �܂菈�����e�ɕs��������A�r���Ŏ~�܂�����A�ǂݍ��܂�Ȃ�������ł͂Ȃ�

    // �I���W�i���̂ݐڐG�������Ƀ_���[�W���󂯂��B�܂�A���I�ɃC���X�^���X���琶���������̂Ƃ͈Ⴄ
    // Prefab�Ɏ��O�ɐݒ肵�Ă����ABake�Ŏ��t����̂ł͂Ȃ��A�������ɐ���������K�v��Bake������H
    // ����ABake��Object���K�v�Ȕ��B
    // �܂��A�I���W�i���Ɋւ��Ă��ڐG���ɑ̗͂����������̂̃G���[����������
    // ����A����͈Ⴄ�B�����Ȃ��B�I���W�i���ɐڐG�������ƂŃI���W�i���������Đ������ɎQ�Ƃł��Ȃ�����



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

    public class BulletBaker : Baker<BulletAuthoring>
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

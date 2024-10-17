using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static BulletHelper;
using static EnemyHelper;
using static HealthHelper;

/// <summary>
/// �e�̐ݒ�
/// </summary>
public class BulletAuthoring : MonoBehaviour
{
    [SerializeField, Min(0.0f), Header("�_���[�W��")]
    private float _damageAmount = 0.0f;

    [SerializeField, Header("�w�c�̎��")]
    private CampsType _campsType = 0;

    /// <summary>
    /// �_���[�W��
    /// </summary>
    public float DamageAmount => _damageAmount;

    /// <summary>
    /// �w�c�̎��
    /// </summary>
    public CampsType CampsType => _campsType;

    public class Baker : Baker<BulletAuthoring>
    {
        public override unsafe void Bake(BulletAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new BulletTag());
            AddComponent(entity, new BulletIDealDamageData(src.DamageAmount, src.CampsType));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static EnemyHelper;
using static PlayerHelper;

/// <summary>
/// �G�̐ݒ�
/// </summary>
public class EnemyAuthoring : MonoBehaviour
{
    [SerializeField, Min(0.0f), Header("�ő�̗�")]
    private float _maxHP = 0.0f;

    [SerializeField, Min(0.0f), Header("���G���Ԃ̒���")]
    private float _isInvincibleTime = 0.0f;

    /// <summary>
    /// ���G���Ԃ̒���
    /// </summary>
    public float IsInvincibleTime => _isInvincibleTime;

    /// <summary>
    /// �ő�̗�
    /// </summary>
    public float MaxHP => _maxHP;

    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Data���A�^�b�`
            AddComponent(entity, new EnemyTag());
            AddComponent(entity, new DestroyableData());
            AddComponent(entity, new EnemyHealthPointData(src.MaxHP, src.IsInvincibleTime));
        }
    }
}

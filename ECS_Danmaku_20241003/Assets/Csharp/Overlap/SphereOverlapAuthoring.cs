using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// ����̏d�Ȃ�����m����ׂ̏��
/// </summary>
public struct SphereOverlapData : IComponentData
{
    public readonly float radius;

    // Enter
    // Exit
    // Stay

    // ��L�O�̓��A�ǂ��L���ɂ��邩���C���X�y�N�^����I�ׂ�悤�ɂ���
    // ���ꂼ���Action��R����ꍇ�A����A�Ⴄ�A�O��Action������Anull�̂��͔̂�L�����ƌ��Ȃ�
    // EnterAction��null�łȂ����Enter���L���ɂȂ�Bnull�͖����ɂȂ�B
    // Action��null�����e���Ă��邩��g���Ȃ�����...

    /// <summary>
    /// ����̏d�Ȃ�����m����ׂ̏��
    /// </summary>
    public SphereOverlapData(float radius)
    {
        this.radius = radius;
    }
}

/// <summary>
/// ����̏d�Ȃ�����m����ׂ̃C���X�y�N�^�ݒ�
/// </summary>
public class SphereOverlapAuthoring : MonoBehaviour
{
    [SerializeField, Min(0.0f), Header("���a")]
    private float _radius = 0.0f;

    /// <summary>
    /// ���a
    /// </summary>
    public float Radius => _radius;

    /// <summary>
    /// SphereOverlapAuthoring��Bake
    /// </summary>
    public class Baker : Baker<SphereOverlapAuthoring>
    {
        public override void Bake(SphereOverlapAuthoring src)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new SphereOverlapData(src.Radius));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static VFXCreationBridge;
using UnityEngine.UIElements;


#if UNITY_EDITOR
using System.Runtime.InteropServices.ComTypes;
using static BulletHelper;
#endif

/// <summary>
/// VFXCreation�̈����̏��
/// </summary>
public struct VFXCreationData : IComponentData
{
    public readonly VisualEffectName visualEffectName;
    public readonly float size;

    private float3 position;
    private bool isCreatable;

    /// <summary>
    /// �����ʒu�B<br/>
    /// x,y,z�A�S�ĂɕύX���������ꍇ�Ɂu�����\�v�̃t���O�𗧂Ă�B
    /// </summary>
    public float3 Position
    {
        get => position;
        set
        {
            // x,y,z�A�S�ĂɕύX���������ꍇ�Ɂu�����\�v�̃t���O�𗧂Ă�
            if (position.x != value.x && position.y != value.y && position.z != value.z)
            {
                position = value;
                isCreatable = true;
            }
        }
    }

    /// <summary>
    /// �����\���ǂ����̃t���O
    /// </summary>
    public bool IsCreatable => isCreatable;

    /// <summary>
    /// VFXCreation�̈����̏��
    /// </summary>
    /// <param name="visualEffectName">VisualEffect�̖���</param>
    /// <param name="size">�傫��</param>
    public VFXCreationData(VisualEffectName visualEffectName, float size)
    {
        this.visualEffectName = visualEffectName;
        this.size = size;

        position = new float3
            (
                float.NegativeInfinity,
                float.NegativeInfinity,
                float.NegativeInfinity
            );
        isCreatable = false;
    }

    /// <summary>
    /// �����𖳌��ɂ���
    /// </summary>
    public void DisableCreation()
    {
        position = new float3
            (
                float.NegativeInfinity,
                float.NegativeInfinity,
                float.NegativeInfinity
            );
        isCreatable = false;
    }
}

public class VFXCreationAuthoring : MonoBehaviour
{
    private const float MIN_SIZE = 0.1f;

    [SerializeField, Header("VFX�̖���")]
    private VisualEffectName _visualEffectName = 0;

    [SerializeField, Min(MIN_SIZE), Header("VFX�̑傫��")]
    private float _size = MIN_SIZE;

    public class Baker : Baker<VFXCreationAuthoring>
    {
        public override void Bake(VFXCreationAuthoring src)
        {
            var enemy = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(enemy, new VFXCreationData(src._visualEffectName, src._size));
        }
    }
}

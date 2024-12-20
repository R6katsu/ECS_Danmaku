using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
using static VFXCreationBridge;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using static BulletHelper;
#endif

// リファクタリング済み

/// <summary>
/// VFXCreationの引数の情報
/// </summary>
public struct VFXCreationData : IComponentData
{
    public readonly VisualEffectType visualEffectName;
    public readonly float size;

    private float3 position;
    private bool isCreatable;

    /// <summary>
    /// 生成位置。<br/>
    /// x,y,z、全てに変更があった場合に「生成可能」のフラグを立てる。
    /// </summary>
    public float3 Position
    {
        get => position;
        set
        {
            // x,y,z、全てに変更があった場合に「生成可能」のフラグを立てる
            if (position.x != value.x && position.y != value.y && position.z != value.z)
            {
                position = value;
                isCreatable = true;
            }
        }
    }

    /// <summary>
    /// 生成可能かどうかのフラグ
    /// </summary>
    public bool IsCreatable => isCreatable;

    /// <summary>
    /// VFXCreationの引数の情報
    /// </summary>
    /// <param name="visualEffectName">VisualEffectの名称</param>
    /// <param name="size">大きさ</param>
    public VFXCreationData(VisualEffectType visualEffectName, float size)
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
    /// 生成を無効にする
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

/// <summary>
/// VFXCreationの引数に必要な設定
/// </summary>
public class VFXCreationAuthoring : MonoBehaviour
{
    private const float MIN_SIZE = 0.1f;

    [SerializeField, Header("VFXの名称")]
    private VisualEffectType _visualEffectName = 0;

    [SerializeField, Min(MIN_SIZE), Header("VFXの大きさ")]
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

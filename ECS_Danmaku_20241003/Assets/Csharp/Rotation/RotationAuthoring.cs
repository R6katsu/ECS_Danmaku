using System;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// ©g‚Ì‰ñ“]‚Ìî•ñ
/// </summary>
[Serializable]
public struct RotationData : IComponentData
{
    [Header("‰ñ“]•ûŒü"), Tooltip("‰ñ“]•ûŒü")]
    public AxisType axisType;

    [Header("‰ñ“]‘¬“xi•‰‚Ì’l‚Í‹t‰ñ“]j"), Tooltip("‰ñ“]‘¬“xi•‰‚Ì’l‚Í‹t‰ñ“]j")]
    public float rotationSpeed;
}

/// <summary>
/// ©g‚Ì‰ñ“]‚Ìİ’è
/// </summary>
public class RotationAuthoring : MonoBehaviour
{
    [SerializeField, Header("©g‚Ì‰ñ“]‚Ìî•ñ")]
    private RotationData _rotationData = new();

    public class Baker : Baker<RotationAuthoring>
    {
        public override void Bake(RotationAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            // ©g‚Ì‰ñ“]‚Ìî•ñ‚ğƒAƒ^ƒbƒ`
            AddComponent(entity, src._rotationData);
        }
    }
}

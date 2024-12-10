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
    [SerializeField, Header("‰ñ“]•ûŒü")]
    private AxisType _axisType;

    [SerializeField, Header("‰ñ“]‘¬“xi•‰‚Ì’l‚Í‹t‰ñ“]j")]
    private float _rotationSpeed;

    /// <summary>
    /// ‰ñ“]•ûŒü
    /// </summary>
    public AxisType AxisType => _axisType;

    /// <summary>
    /// ‰ñ“]‘¬“xi•‰‚Ì’l‚Í‹t‰ñ“]j
    /// </summary>
    public float RotationSpeed => _rotationSpeed;

    /// <summary>
    /// ©g‚Ì‰ñ“]‚Ìî•ñ
    /// </summary>
    /// <param name="axisType">‰ñ“]•ûŒü</param>
    /// <param name="rotationSpeed">‰ñ“]‘¬“xi•‰‚Ì’l‚Í‹t‰ñ“]j</param>
    public RotationData(AxisType axisType, float rotationSpeed)
    {
        _axisType = axisType;
        _rotationSpeed = rotationSpeed;
    }
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

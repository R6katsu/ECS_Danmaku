using System;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// 自身の回転の情報
/// </summary>
[Serializable]
public struct RotationData : IComponentData
{
    [SerializeField, Header("回転方向")]
    private AxisType _axisType;

    [SerializeField, Header("回転速度（負の値は逆回転）")]
    private float _rotationSpeed;

    /// <summary>
    /// 回転方向
    /// </summary>
    public AxisType AxisType => _axisType;

    /// <summary>
    /// 回転速度（負の値は逆回転）
    /// </summary>
    public float RotationSpeed => _rotationSpeed;

    /// <summary>
    /// 自身の回転の情報
    /// </summary>
    /// <param name="axisType">回転方向</param>
    /// <param name="rotationSpeed">回転速度（負の値は逆回転）</param>
    public RotationData(AxisType axisType, float rotationSpeed)
    {
        _axisType = axisType;
        _rotationSpeed = rotationSpeed;
    }
}

/// <summary>
/// 自身の回転の設定
/// </summary>
public class RotationAuthoring : MonoBehaviour
{
    [SerializeField, Header("自身の回転の情報")]
    private RotationData _rotationData = new();

    public class Baker : Baker<RotationAuthoring>
    {
        public override void Bake(RotationAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            // 自身の回転の情報をアタッチ
            AddComponent(entity, src._rotationData);
        }
    }
}

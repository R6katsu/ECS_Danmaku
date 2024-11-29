using System;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// 自身の回転の情報
/// </summary>
[Serializable]
public struct RotationData : IComponentData, IEnumerable, IDataDeletion
{
    [Header("回転方向"), Tooltip("回転方向")]
    public AxisType axisType;

    [Header("回転速度（負の値は逆回転）"), Tooltip("回転速度（負の値は逆回転）")]
    public float rotationSpeed;

    public bool IsDataDeletion { get; set; }

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
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

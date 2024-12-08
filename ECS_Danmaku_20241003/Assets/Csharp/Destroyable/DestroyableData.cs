using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// Entityの削除に必要なフラグ情報
/// </summary>
public struct DestroyableData : IComponentData
{
    [Tooltip("削除フラグ")]
    public bool isKilled;
}

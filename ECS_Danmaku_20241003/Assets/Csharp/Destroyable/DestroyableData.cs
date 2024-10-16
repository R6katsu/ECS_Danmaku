using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// 削除に関する情報
/// </summary>
public struct DestroyableData : IComponentData
{
    [Tooltip("削除フラグ")]
    public bool isKilled;
}

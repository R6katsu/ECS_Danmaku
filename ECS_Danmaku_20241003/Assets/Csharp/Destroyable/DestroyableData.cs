using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// �폜�Ɋւ�����
/// </summary>
public struct DestroyableData : IComponentData
{
    [Tooltip("�폜�t���O")]
    public bool isKilled;
}

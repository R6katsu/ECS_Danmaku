using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// Entity�̍폜�ɕK�v�ȃt���O���
/// </summary>
public struct DestroyableData : IComponentData
{
    [Tooltip("�폜�t���O")]
    public bool isKilled;
}

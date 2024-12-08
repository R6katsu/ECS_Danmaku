using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �e���̎��
/// </summary>
public enum DanmakuType : byte
{
    [Tooltip("�����l")] None,
    [Tooltip("n-Way�e")] N_Way,
    [Tooltip("�^�b�v����")] TapShooting
}

/// <summary>
/// �e���̕⏕
/// </summary>
static public class DanmakuHelper
{
    /// <summary>
    /// �e���̐ݒ������
    /// </summary>
    public interface IDanmakuAuthoring { }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// �e���̕⏕
/// </summary>
static public class DanmakuHelper
{
    /// <summary>
    /// �e���̎��
    /// </summary>
    public enum DanmakuType
    {
        None,
        [Tooltip("n-Way�e")] N_Way,
        [Tooltip("�^�b�v����")] TapShooting
    }

    /// <summary>
    /// �e���̐ݒ������
    /// </summary>
    public interface IDanmakuAuthoring { }
}

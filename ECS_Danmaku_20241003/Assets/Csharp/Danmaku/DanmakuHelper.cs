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
        N_Way,
    }

    /// <summary>
    /// �e���̐ݒ������
    /// </summary>
    public interface IDanmakuAuthoring { }
}

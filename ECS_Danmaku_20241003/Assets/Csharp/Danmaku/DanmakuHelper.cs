using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
#endif

// t@N^OÏÝ

/// <summary>
/// eÌíÞ
/// </summary>
public enum DanmakuType : byte
{
    [Tooltip("úl")] None,
    [Tooltip("n-Waye")] N_Way,
    [Tooltip("^bv¿")] TapShooting
}

/// <summary>
/// eÌâ
/// </summary>
static public class DanmakuHelper
{
    /// <summary>
    /// eÌÝèðÀ
    /// </summary>
    public interface IDanmakuAuthoring { }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// eÌâ
/// </summary>
static public class DanmakuHelper
{
    /// <summary>
    /// eÌíÞ
    /// </summary>
    public enum DanmakuType
    {
        None,
        [Tooltip("n-Waye")] N_Way,
        [Tooltip("^bv¿")] TapShooting
    }

    /// <summary>
    /// eÌÝèðÀ
    /// </summary>
    public interface IDanmakuAuthoring { }
}

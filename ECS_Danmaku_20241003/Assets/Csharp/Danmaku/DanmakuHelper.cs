using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// ’e–‹‚Ì•â•
/// </summary>
static public class DanmakuHelper
{
    /// <summary>
    /// ’e–‹‚Ìí—Ş
    /// </summary>
    public enum DanmakuType
    {
        None,
        [Tooltip("n-Way’e")] N_Way,
        [Tooltip("ƒ^ƒbƒvŒ‚‚¿")] TapShooting
    }

    /// <summary>
    /// ’e–‹‚Ìİ’è‚ğÀ‘•
    /// </summary>
    public interface IDanmakuAuthoring { }
}

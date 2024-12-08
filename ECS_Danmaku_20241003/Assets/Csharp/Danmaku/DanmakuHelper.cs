using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
#endif

// リファクタリング済み

/// <summary>
/// 弾幕の種類
/// </summary>
public enum DanmakuType : byte
{
    [Tooltip("初期値")] None,
    [Tooltip("n-Way弾")] N_Way,
    [Tooltip("タップ撃ち")] TapShooting
}

/// <summary>
/// 弾幕の補助
/// </summary>
static public class DanmakuHelper
{
    /// <summary>
    /// 弾幕の設定を実装
    /// </summary>
    public interface IDanmakuAuthoring { }
}

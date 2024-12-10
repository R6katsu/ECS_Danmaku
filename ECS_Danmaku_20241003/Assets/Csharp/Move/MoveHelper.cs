using System;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// 移動の補助
/// </summary>
static public class MoveHelper
{
    /// <summary>
    /// 移動
    /// </summary>
    public struct MoveTag : IComponentData { }

    /// <summary>
    /// 移動に必要な設定値
    /// </summary>
    [Serializable]
    public struct MoveParameter
    {
        [SerializeField, Header("移動速度")]
        private float _speed;

        /// <summary>
        /// 移動速度
        /// </summary>
        public float Speed => _speed;
    }
}

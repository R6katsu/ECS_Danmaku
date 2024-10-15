using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 移動の補助
/// </summary>
static public class MoveHelper
{
    /// <summary>
    /// 移動方向の種類
    /// </summary>
    public enum DirectionTravelType
    {
        [Tooltip("停止")] Stop,
        [Tooltip("上")] Up,
        [Tooltip("下")] Down,
        [Tooltip("左")] Left,
        [Tooltip("右")] Right,
        [Tooltip("前")] Forward,
        [Tooltip("後")] Back
    }

    /// <summary>
    /// DirectionTravelTypeを移動方向に変換する
    /// </summary>
    static public class DirectionTravelTypeConverter
    {
        /// <summary>
        /// DirectionTravelTypeを移動方向に変換する
        /// </summary>
        /// <param name="directionTravelType">移動方向の種類</param>
        /// <returns>DirectionTravelTypeに応じた移動方向</returns>
        static public float3 GetDirectionVector(DirectionTravelType directionTravelType)
        {
            switch (directionTravelType)
            {
                case DirectionTravelType.Stop:
                default:
                    return Vector3.zero;

                case DirectionTravelType.Up:
                    return Vector3.up;

                case DirectionTravelType.Down:
                    return Vector3.down;

                case DirectionTravelType.Left:
                    return Vector3.left;

                case DirectionTravelType.Right:
                    return Vector3.right;

                case DirectionTravelType.Forward:
                    return Vector3.forward;

                case DirectionTravelType.Back:
                    return Vector3.back;
            }
        }
    }

    /// <summary>
    /// 移動
    /// </summary>
    public struct MoveTag : IComponentData { }

    /// <summary>
    /// 移動方向の種類の情報
    /// </summary>
    public struct DirectionTravelData : IComponentData
    {
        [Tooltip("移動方向の種類")]
        public readonly DirectionTravelType directionTravelType;

        /// <summary>
        /// 移動方向の種類の情報
        /// </summary>
        /// <param name="directionTravelType">移動方向の種類</param>
        public DirectionTravelData(DirectionTravelType directionTravelType)
        {
            this.directionTravelType = directionTravelType;
        }
    }

    /// <summary>
    /// 移動に必要な設定値
    /// </summary>
    [Serializable]
    public struct MoveParameter
    {
        [SerializeField, Min(0.0f)]
        private float _speed;

        /// <summary>
        /// 移動速度
        /// </summary>
        public float Speed => _speed;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

static public class MoveHelper
{
    /// <summary>
    /// 移動方向の種類
    /// </summary>
    public enum DirectionTravelType
    {
        Stop,   // 停止
        Up,     // 上
        Down,   // 下
        Left,   // 左
        Right   // 右
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
            }
        }
    }

    public struct MoveTag : IComponentData { }

    public struct DirectionTravelData : IComponentData
    {
        public readonly DirectionTravelType directionTravelType;

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

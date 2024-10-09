using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

static public class MoveHelper
{
    /// <summary>
    /// �ړ������̎��
    /// </summary>
    public enum DirectionTravelType
    {
        Stop,   // ��~
        Up,     // ��
        Down,   // ��
        Left,   // ��
        Right   // �E
    }

    /// <summary>
    /// DirectionTravelType���ړ������ɕϊ�����
    /// </summary>
    static public class DirectionTravelTypeConverter
    {
        /// <summary>
        /// DirectionTravelType���ړ������ɕϊ�����
        /// </summary>
        /// <param name="directionTravelType">�ړ������̎��</param>
        /// <returns>DirectionTravelType�ɉ������ړ�����</returns>
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
    /// �ړ��ɕK�v�Ȑݒ�l
    /// </summary>
    [Serializable]
    public struct MoveParameter
    {
        [SerializeField, Min(0.0f)]
        private float _speed;

        /// <summary>
        /// �ړ����x
        /// </summary>
        public float Speed => _speed;
    }
}

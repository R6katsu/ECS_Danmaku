using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// �ړ��̕⏕
/// </summary>
static public class MoveHelper
{
    /// <summary>
    /// �ړ������̎��
    /// </summary>
    public enum DirectionTravelType
    {
        [Tooltip("��~")] Stop,
        [Tooltip("��")] Up,
        [Tooltip("��")] Down,
        [Tooltip("��")] Left,
        [Tooltip("�E")] Right,
        [Tooltip("�O")] Forward,
        [Tooltip("��")] Back
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

                case DirectionTravelType.Forward:
                    return Vector3.forward;

                case DirectionTravelType.Back:
                    return Vector3.back;
            }
        }
    }

    /// <summary>
    /// �ړ�
    /// </summary>
    public struct MoveTag : IComponentData { }

    /// <summary>
    /// �ړ������̎�ނ̏��
    /// </summary>
    public struct DirectionTravelData : IComponentData
    {
        [Tooltip("�ړ������̎��")]
        public readonly DirectionTravelType directionTravelType;

        /// <summary>
        /// �ړ������̎�ނ̏��
        /// </summary>
        /// <param name="directionTravelType">�ړ������̎��</param>
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

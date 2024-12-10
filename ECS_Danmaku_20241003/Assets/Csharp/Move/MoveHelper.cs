using System;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �ړ��̕⏕
/// </summary>
static public class MoveHelper
{
    /// <summary>
    /// �ړ�
    /// </summary>
    public struct MoveTag : IComponentData { }

    /// <summary>
    /// �ړ��ɕK�v�Ȑݒ�l
    /// </summary>
    [Serializable]
    public struct MoveParameter
    {
        [SerializeField, Header("�ړ����x")]
        private float _speed;

        /// <summary>
        /// �ړ����x
        /// </summary>
        public float Speed => _speed;
    }
}

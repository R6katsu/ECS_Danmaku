using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;
using static MoveHelper;

#if UNITY_EDITOR
using NUnit.Framework;
using System.Security.Cryptography;
using Unity.Core;
#endif

/// <summary>
/// �G�̕⏕
/// </summary>
static public class EnemyHelper
{
    /// <summary>
    /// �G
    /// </summary>
    public struct EnemyTag : IComponentData{ }

    /// <summary>
    /// �G�̐����ɕK�v�ȏ��
    /// </summary>
    public struct EnemySpawnerData : IComponentData
    {
        [Tooltip("��������Prefab��Entity")]
        public readonly Entity enemyEntity;

        [Tooltip("������̈ړ�����")]
        public readonly DirectionTravelType directionTravelType;
        
        [Tooltip("����������̎n�_")]
        public readonly Vector3 startPoint;

        [Tooltip("����������̏I�_")]
        public readonly Vector3 endPoint;

        /// <summary>
        /// �G�̐����ɕK�v�ȏ��
        /// </summary>
        /// <param name="enemyEntity">��������Prefab��Entity</param>
        /// <param name="directionTravelType">������̈ړ�����</param>
        /// <param name="startPoint">����������̎n�_</param>
        /// <param name="endPoint">����������̏I�_</param>
        public EnemySpawnerData(
            Entity enemyEntity,
            DirectionTravelType directionTravelType,
            Vector3 startPoint, 
            Vector3 endPoint)
        {
            this.enemyEntity = enemyEntity;
            this.directionTravelType = directionTravelType;
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }
    }

    public class EnemyHealthPointDataDic
    {
        // ���ƂŏC������B�Ƃ肠�����̓z
        // BurstCompile������t�^���Ă���ꏊ����Ăяo�����ꍇ�̓G���[�ɂȂ�

        static public Dictionary<int, List<Entity>> entitys = new();
    }
}

using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.Collections;
using static HealthHelper;
using static MoveHelper;
using NUnit.Framework;
using System.Security.Cryptography;
using Unity.Core;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �GEntity�̖���
/// </summary>
public enum EnemyName : byte
{
    [Tooltip("�S���ʒe")] AllDirectionBulletEnemy,
    [Tooltip("��]����")] SpinShootingEnemy,
}

/// <summary>
/// �G�̕⏕
/// </summary>
static public class EnemyHelper
{
    /// <summary>
    /// �G
    /// </summary>
    public struct EnemyTag : IComponentData{ }
}

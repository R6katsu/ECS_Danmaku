using UnityEngine;

#if UNITY_EDITOR
using Unity.Entities;
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
/// �GEntity�̎��
/// </summary>
public enum EnemyType : byte
{
    [Tooltip("�S���ʒe")] AllDirectionBulletEnemy,
    [Tooltip("��]����")] SpinShootingEnemy,
}
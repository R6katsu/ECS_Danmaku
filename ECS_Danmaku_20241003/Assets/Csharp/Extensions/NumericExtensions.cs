using UnityEngine;
using Unity.Mathematics;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// ���l�n�̊g���֐�
/// </summary>
public static class NumericExtensions
{
    [Tooltip("�����̒l")]
    private const int HALVE_NUMBER = 2;

    /// <summary>
    /// int�̊g���֐��B<br/>
    /// �����̒l��Ԃ�
    /// </summary>
    /// <param name="value">�����Ɋ���l</param>
    /// <returns>�����̒l</returns>
    public static int Halve(this int value)
    {
        return value / HALVE_NUMBER;
    }

    /// <summary>
    /// int2�̊g���֐��B<br/>
    /// �����̒l��Ԃ�
    /// </summary>
    /// <param name="value">�����Ɋ���l</param>
    /// <returns>�����̒l</returns>
    public static int2 Halve(this int2 value)
    {
        return value / HALVE_NUMBER;
    }

    /// <summary>
    /// int3�̊g���֐��B<br/>
    /// �����̒l��Ԃ�
    /// </summary>
    /// <param name="value">�����Ɋ���l</param>
    /// <returns>�����̒l</returns>
    public static int3 Halve(this int3 value)
    {
        return value / HALVE_NUMBER;
    }

    /// <summary>
    /// float�̊g���֐��B<br/>
    /// �����̒l��Ԃ�
    /// </summary>
    /// <param name="value">�����Ɋ���l</param>
    /// <returns>�����̒l</returns>
    public static float Halve(this float value)
    {
        return value / HALVE_NUMBER;
    }

    /// <summary>
    /// float2�̊g���֐��B<br/>
    /// �����̒l��Ԃ�
    /// </summary>
    /// <param name="value">�����Ɋ���l</param>
    /// <returns>�����̒l</returns>
    public static float2 Halve(this float2 value)
    {
        return value / HALVE_NUMBER;
    }

    /// <summary>
    /// float3�̊g���֐��B<br/>
    /// �����̒l��Ԃ�
    /// </summary>
    /// <param name="value">�����Ɋ���l</param>
    /// <returns>�����̒l</returns>
    public static float3 Halve(this float3 value)
    {
        return value / HALVE_NUMBER;
    }
}

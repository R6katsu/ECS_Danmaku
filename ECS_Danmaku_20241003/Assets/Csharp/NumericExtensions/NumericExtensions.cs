using UnityEngine;
using Unity.Mathematics;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// ���l�̊g���֐�
/// </summary>
public static class NumericExtensions
{
    [Tooltip("�����̒l")]
    private const int HALVE_NUMBER = 2;

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

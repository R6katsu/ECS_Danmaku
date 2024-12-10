using UnityEngine;
using Unity.Mathematics;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// 数値の拡張関数
/// </summary>
public static class NumericExtensions
{
    [Tooltip("半分の値")]
    private const int HALVE_NUMBER = 2;

    /// <summary>
    /// floatの拡張関数。<br/>
    /// 半分の値を返す
    /// </summary>
    /// <param name="value">半分に割る値</param>
    /// <returns>半分の値</returns>
    public static float Halve(this float value)
    {
        return value / HALVE_NUMBER;
    }

    /// <summary>
    /// float3の拡張関数。<br/>
    /// 半分の値を返す
    /// </summary>
    /// <param name="value">半分に割る値</param>
    /// <returns>半分の値</returns>
    public static float3 Halve(this float3 value)
    {
        return value / HALVE_NUMBER;
    }
}

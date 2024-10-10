using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// 球状の重なりを感知する為の情報
/// </summary>
public struct SphereOverlapData : IComponentData
{
    public readonly float radius;

    // Enter
    // Exit
    // Stay

    // 上記三つの内、どれを有効にするかをインスペクタから選べるようにする
    // それぞれにActionを紐つける場合、いや、違う、三つActionがあり、nullのものは非有効化と見なす
    // EnterActionがnullでなければEnterが有効になる。nullは無効になる。
    // Actionはnullを許容しているから使えないかも...

    /// <summary>
    /// 球状の重なりを感知する為の情報
    /// </summary>
    public SphereOverlapData(float radius)
    {
        this.radius = radius;
    }
}

/// <summary>
/// 球状の重なりを感知する為のインスペクタ設定
/// </summary>
public class SphereOverlapAuthoring : MonoBehaviour
{
    [SerializeField, Min(0.0f), Header("半径")]
    private float _radius = 0.0f;

    /// <summary>
    /// 半径
    /// </summary>
    public float Radius => _radius;

    /// <summary>
    /// SphereOverlapAuthoringのBake
    /// </summary>
    public class Baker : Baker<SphereOverlapAuthoring>
    {
        public override void Bake(SphereOverlapAuthoring src)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new SphereOverlapData(src.Radius));
        }
    }
}

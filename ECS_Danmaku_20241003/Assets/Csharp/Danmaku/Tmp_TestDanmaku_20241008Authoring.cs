using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static DanmakuHelper;

// 弾を放つ敵に必要なTagを作成
// 弾幕の種類によって付けるTagが違う？
// 弾幕Tagを持たせ、enumを保持させてもいいかも。それならTagの数を減らせる
// インスペクタからenumを選び、Data化する時にTagをアタッチする

public struct Tmp_TestDanmaku_20241008Data : IComponentData
{
    public readonly DanmakuType danmakuType;
    public readonly Entity bulletPrefab;

    public Tmp_TestDanmaku_20241008Data(DanmakuType danmakuType, Entity bulletPrefab)
    {
        this.danmakuType = danmakuType;
        this.bulletPrefab = bulletPrefab;
    }
}

public class Tmp_TestDanmaku_20241008Authoring : MonoBehaviour
{
    // 弾幕ごとに間隔や角度などを決められるようにする
    // 弾幕ごとに「この範囲を出たら削除」というAABBを決められるようにする

    [SerializeField]
    private DanmakuType _danmakuType = 0;

    [SerializeField]
    private DanmakuTypeSO _danmakuTypeSO = null;

    // 弾幕と使用する弾Prefabの設定をSOで行い、それを配列にインスペクタから代入するのはどうだろうか

    /// <summary>
    /// 弾幕の種類
    /// </summary>
    private DanmakuType MyDanmakuType => _danmakuType;

    private DanmakuTypeSO DanmakuTypeSO => _danmakuTypeSO;

    public class Baker : Baker<Tmp_TestDanmaku_20241008Authoring>
    {
        public override void Bake(Tmp_TestDanmaku_20241008Authoring src)
        {
            var bulletEntity = GetEntity(src.DanmakuTypeSO.BulletPrefab, TransformUsageFlags.Dynamic);

            var tmp_TestDanmaku_20241008 = new Tmp_TestDanmaku_20241008Data(src.MyDanmakuType, bulletEntity);

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), tmp_TestDanmaku_20241008);
        }
    }
}

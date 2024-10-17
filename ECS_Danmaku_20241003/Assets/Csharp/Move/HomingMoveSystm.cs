using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// 対象に向かって直線移動する処理
/// </summary>
[BurstCompile]
public partial struct HomingMoveSystm : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        // 対象はどうやって指定する？
        // ここでは対象に向かって直線移動するだけ
        // 対象は他の場所で指定する
        // いや、特に指定がない場合は自身と異なる陣営の中から一番近い対象を狙えばいいか

        // campsType

        // 対象に向かって直線移動する処理
        foreach (var (homing, localTfm) in
                 SystemAPI.Query<RefRO<HomingMoveData>,
                                 RefRW<LocalTransform>>())
        {
            // 対象がnullだった
            if (homing.ValueRO.targetEntity == Entity.Null)
            {
                // 最も近い異なる陣営のEntityを対象とする

                // 普通に考えて、全てのEntityから陣営を取得して距離を計算するって、
                // 全ての追尾弾が処理したら重くなるよね
                // 陣営毎にListを作成する？　弾と敵で対象が違う筈、それも考慮する
                // 他の場所で陣営毎のEntityを管理し、それを探索して距離を求める？
                // 弾か、敵かで対象も異なる。Moveだけでなく、敵か弾かもenumか何かで判別したい
                // 弾、障害物、敵（生物）でTag分けするといいかも
                // 今回は生物+異なる陣営を順番に探索する必要がある
                // 陣営もenumではなくDataにする（Tag）。enumはあくまでもインスペクタから設定する時だけ使用する
            }

            // ローカル座標系を使い、向いている方向への移動を計算
            //float3 forward = math.forward(localTfm.ValueRO.Rotation);

            // 移動を反映
            //localTfm.ValueRW.Position += forward * facingDirection.ValueRO.moveParam.Speed * delta;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;
using static EntityCampsHelper;
using static EntityCategoryHelper;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 対象に向かって直線移動する処理
/// </summary>
[BurstCompile]
public partial struct HomingMoveSystm : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        Debug.LogError("ここに予定を書いている");

        // 敵に弾が当たった時、弾によっては消えるようにする
        // 貫通などをenumで決めるか。それとも回数？


        // ディレクトリについての#ifを書いて軽量化する
        // Authoringでgetプロパティを無くしてprivateだけにする
        // 陣営をAuthoringで設定するのではなく、引数として入れてもらう？
        // いや、自分の陣営がアンノウンだった場合は自身の所属するEntityから陣営Dataを取得して代入するように設計する
        // 陣営だけではなく、Entityの種類（生物、武器、弾幕、障害物etc...）についてのTagも実装する

        // Tagについては、自身の所属するEntityからDataを取得する方法を考える



        // 対象はどうやって指定する？
        // ここでは対象に向かって直線移動するだけ
        // 対象は他の場所で指定する
        // いや、特に指定がない場合は自身と異なる陣営の中から一番近い対象を狙えばいいか

        // campsType

        // 対象に向かって直線移動する処理
        foreach (var (homing, velocity, localTfm) in
                 SystemAPI.Query<RefRO<HomingMoveData>,
                                 RefRW<PhysicsVelocity>,
                                 RefRW<LocalTransform>>())
        {
            //continue;   // 未完成の為

            // 対象がnullだった
            if (homing.ValueRO.targetEntity == Entity.Null)
            {
                // 取得したEntityを使った処理は全て同じ
                // 距離を計算して一番近い相手を目指せばいい。これは共通
                // もっと言えば、Entityではなく、対応するTagを持った奴の位置情報が欲しいだけ

                // いや、そのEntityが持つ全てのDataを取得してから比較するのもなんだかなぁって感じ
                // やっぱり一度で陣営TagとカテゴリTagを一致させたい
                // まあいい。とりあえず敵に向かう弾幕を完成させよう

                float maxjpjfaiopfjaiop = float.MaxValue;
                float3 gjniknhiaogkvihaoTfm = new float3();    // 一番近い
                float3 currentPoint = localTfm.ValueRO.Position;

                if (homing.ValueRO.targetCampsType == EntityCampsType.Enemy && homing.ValueRO.targetEntityCategory == EntityCategory.LivingCreature)
                {
                    foreach (var (livingCreatureCategory, enemyCamps, targetLocalTfm) in
                    SystemAPI.Query<RefRO<LivingCreatureCategoryTag>,
                                    RefRO<EnemyCampsTag>,
                                    RefRW<LocalTransform>>())
                    {
                        // 目標が生物 & 敵陣営だった

                        var targetPoint = targetLocalTfm.ValueRO.Position;
                        var dis = math.distance(targetPoint, currentPoint);

                        // 最も近いとされる距離を更新した
                        if (dis < maxjpjfaiopfjaiop)
                        {
                            maxjpjfaiopfjaiop = dis;
                            gjniknhiaogkvihaoTfm = targetPoint;
                        }
                    }
                }
                else
                {
                    Debug.Log("未実装");
                }

                // 現在位置から目標位置へのベクトルを計算
                float3 direction = math.normalize(gjniknhiaogkvihaoTfm - currentPoint);

                /*
                // 移動する距離を計算
                float distanceToMove = homing.ValueRO.moveParam.Speed * delta;

                // 移動を適用
                localTfm.ValueRW.Position += direction * distanceToMove;
                */

                // なんというか、余韻みたいなのを持たせたい

                // ターゲットの位置を取得
                float3 targetPosition = gjniknhiaogkvihaoTfm;

                // 現在の弾の位置とターゲットの位置との差を計算
                float3 directionToTarget = direction;

                // 現在の弾の速度方向にターゲット方向を加味して、新しい方向へ速度を調整
                float3 newVelocity = math.lerp(velocity.ValueRO.Linear, directionToTarget * homing.ValueRO.moveParam.Speed, homing.ValueRO.moveParam.Speed * delta);

                // 新しい速度を設定
                velocity.ValueRW.Linear = newVelocity;



                // 移動の入力を取得（例：右に移動する場合）
                //velocity.ValueRW.Linear.x = moveSpeed; // x方向に移動
                //velocity.ValueRW.Linear.y = 0f;
                //velocity.ValueRW.Linear.z = 0f;
            }
        }
    }
}

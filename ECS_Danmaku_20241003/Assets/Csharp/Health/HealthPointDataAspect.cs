using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System;
using UnityEngine;
using static EnemyHelper;
using static HealthPointDatas;
using static PlayerHelper;
using static UnityEngine.EventSystems.EventTrigger;
using static HealthHelper;

#if UNITY_EDITOR
#endif

/// <summary>
/// HealthPointDataが纏まったもの
/// </summary>
public readonly partial struct HealthPointDataAspect : IAspect
{
    // BetterHealthPoint

    public readonly RefRW<PlayerHealthPointData> refRW_PlayerHealthPointData;
    public readonly RefRW<EnemyHealthPointData> refRW_EnemyHealthPointData;

    // Field変数については、この変数を実装するというインターフェースを継承する
    // この変数を実装しているというインターフェースを継承していた場合、その変数を使用した処理を持つ関数を呼べる
    // 関数の種類についてはenumで設定する。なんならenumについてもインターフェースで実装し、選択可能な関数のみenumに含めるか

    // HealthPointDataAspectを呼び出せば様々なHPDataを呼び出せるのではないか
    // というか、関数のみを変えてその他の処理を一緒にする方法もできるのではないか
    // 毎回異なるIHealthを継承した構造体を作成する必要がないかもしれない

    // PlayerHealthPointDataではなく、Tagのようなものにする
    // Tagとの違いはフィールド変数があること
    // そのため、Tagという構造体名ではなく他の命名規則を考える
    // 関数の内容だけTagモドキと紐つける

    // 変数がTagもどき毎に異なる（Enemyには無敵時間がない等）為、変数が違う場合は異なる構造体を用意する
    // 無敵時間有構造体と無敵時間無構造体を用意し、更に無敵時間有の中でも被弾時の処理を変えられるようにする
    // 被弾時や回復時の処理はenumか何かで紐つける？enumであればnull非許容なので初期化時に設定できる

    // void Damage() { GetDamage(enum) };

    public enum ASDFGHJKL
    {
        [Tooltip("")] None,
        [Tooltip("")] PlayerDamage
    }

    static public Func<float, Entity, PlayerHealthPointData, PlayerHealthPointData> GetDamage(ASDFGHJKL aSDFGHJKL)
    {
        switch (aSDFGHJKL)
        {
            case ASDFGHJKL.PlayerDamage:
                return PlayerDamage;

            default:
            case ASDFGHJKL.None:
                return null;
        }
    }

    // IHealthPointではなく実装しなければならないフィールド変数のインターフェース
    // いや、その場合は戻り値のActionの最後のインターフェースがそれぞれ異なることになってしまうか
    static public PlayerHealthPointData PlayerDamage(float damage, Entity entity, PlayerHealthPointData playerHealthPointData)
    {
        // なんか複数回呼ばれている？
        // というかHPが9から減らない
        // 情報が更新されていないっぽい
        Debug.Log("PLのダメージ処理");

        Debug.Log("試験的なコードの為、後で直す");
        playerHealthPointData.currentHP -= damage;
        playerHealthPointData.lastHitTime = World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;

        Debug.Log($"{damage}ダメージを受けた。残りHP{playerHealthPointData.currentHP}");

        // HPが 0以下なら倒れる
        if (playerHealthPointData.currentHP <= 0)
            playerHealthPointData.Down();

        return playerHealthPointData;
    }
}

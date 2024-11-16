using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using static BulletHelper;
using static EntityCampsHelper;
using static HealthPointDatas;
using static TriggerJobs;
using EventTrigger = UnityEngine.EventSystems.EventTrigger;

#if UNITY_EDITOR
#endif

/// <summary>
/// タイトルロゴの処理
/// </summary>
//[BurstCompile]
public partial struct TitleLogoSystem : ISystem
{
    private ComponentLookup<TitleLogoSingletonData> _titleLogoSingletonLookup;
    private ComponentLookup<BulletIDealDamageData> _dealDamageLookup;
    private ComponentLookup<DestroyableData> _destroyableLookup;

    public void OnCreate(ref SystemState state)
    {
        // 取得する
        _titleLogoSingletonLookup = state.GetComponentLookup<TitleLogoSingletonData>(false);
        _dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false);
        _destroyableLookup = state.GetComponentLookup<DestroyableData>(false);
    }

    public void OnUpdate(ref SystemState state)
    {
        // 更新する
        _titleLogoSingletonLookup.Update(ref state);
        _dealDamageLookup.Update(ref state);
        _destroyableLookup.Update(ref state);

        // タイトルロゴに弾が当たった時の処理を呼び出す
        var titleLogo = new TitleLogoTriggerJob()
        {
            titleLogoSingletonLookup = _titleLogoSingletonLookup,
            dealDamageLookup = _dealDamageLookup,
            destroyableLookup = _destroyableLookup
        };

        // TitleLogoTriggerJobをスケジュール
        var titleLogoJobHandle = titleLogo.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // ジョブの依存関係を更新する
        state.Dependency = titleLogoJobHandle;

        // ジョブ完了を待機してから次の処理を実行する
        titleLogoJobHandle.Complete();

        foreach (var (titleLogoSingleton, entity) in SystemAPI.Query<TitleLogoSingletonData>().WithEntityAccess())
        {
            // 次の画像に切り替える
            titleLogoSingleton.NextImage();

            // isNextImage の値を実際のエンティティに反映
            state.EntityManager.SetComponentData(entity, titleLogoSingleton);
        }
    }
}

/// <summary>
/// 
/// </summary>
//[BurstCompile]
public partial struct TitleLogoTriggerJob : ITriggerEventsJob
{
    public ComponentLookup<TitleLogoSingletonData> titleLogoSingletonLookup;
    public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
    public ComponentLookup<DestroyableData> destroyableLookup;
    //public ComponentLookup<VFXCreationData> vfxCreationLookup;
    //public ComponentLookup<AudioPlayData> audioPlayLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        var entityA = triggerEvent.EntityA; // 接触対象
        var entityB = triggerEvent.EntityB; // isTriggerを有効にしている方

        // entityAがBulletIDealDamageDataを有しているか
        if (!dealDamageLookup.HasComponent(entityA)) { return; }

        // entityBがTitleLogoSingletonDataを有しているか
        if (!titleLogoSingletonLookup.HasComponent(entityB)) { return; }

        var titleLogoSingleton = titleLogoSingletonLookup[entityB];

        // entityBがDestroyableDataを有しているか
        if (!destroyableLookup.HasComponent(entityA)) { return; }

        // 弾の削除フラグを立てる
        var destroyable = destroyableLookup[entityA];
        destroyable.isKilled = true;
        destroyableLookup[entityA] = destroyable;

        // タイトルロゴの被弾SEを再生
        // メインスレッドから呼び出す為にAudioPlayDataを使う
        //AudioManager.Instance.PlaySE(titleLogoSingleton.damageSENumber);

        // 画像を切り替えるまでの回数をデクリメント
        titleLogoSingleton.currentImageSwapCount--;

        // 画像を切り替えるまでの回数が0より大きかった
        if (titleLogoSingleton.currentImageSwapCount > 0) 
        {
            // 変更を反映
            titleLogoSingletonLookup[entityB] = titleLogoSingleton;
            return;
        }

        titleLogoSingleton.currentImageSwapCount = titleLogoSingleton.imageSwapCount;

        // 画像を切り替える
        titleLogoSingleton.isNextImage = true;

        // 変更を反映
        titleLogoSingletonLookup[entityB] = titleLogoSingleton;
    }
}

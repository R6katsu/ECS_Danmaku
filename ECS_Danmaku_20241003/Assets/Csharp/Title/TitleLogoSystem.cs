using Unity.Entities;
using Unity.Physics;
using static BulletHelper;
using Unity.Burst;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using static EntityCampsHelper;
using static HealthHelper;
using static HealthPointDatas;
using EventTrigger = UnityEngine.EventSystems.EventTrigger;
#endif

// リファクタリング済み

/// <summary>
/// タイトルロゴの処理
/// </summary>
[BurstCompile]
public partial struct TitleLogoSystem : ISystem
{
    private ComponentLookup<TitleLogoSingletonData> _titleLogoSingletonLookup;
    private ComponentLookup<BulletIDealDamageData> _dealDamageLookup;
    private ComponentLookup<DestroyableData> _destroyableLookup;
    private ComponentLookup<AudioPlayData> _audioPlayLookup;
    private ComponentLookup<RemainingPierceCountData> _remainingPierceCountLookup;

    public void OnCreate(ref SystemState state)
    {
        // 取得する
        _titleLogoSingletonLookup = state.GetComponentLookup<TitleLogoSingletonData>(false);
        _dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false);
        _destroyableLookup = state.GetComponentLookup<DestroyableData>(false);
        _audioPlayLookup = state.GetComponentLookup<AudioPlayData>(false);
        _remainingPierceCountLookup = state.GetComponentLookup<RemainingPierceCountData>(false);
    }

    public void OnUpdate(ref SystemState state)
    {
        // タイトルロゴの当たり判定が無効だった
        if (TitleSceneManager.Instance == null 
            || !TitleSceneManager.Instance.HasTitleLogoCollision) { return; }

        // 更新する
        _titleLogoSingletonLookup.Update(ref state);
        _dealDamageLookup.Update(ref state);
        _destroyableLookup.Update(ref state);
        _audioPlayLookup.Update(ref state);
        _remainingPierceCountLookup.Update(ref state);

        // タイトルロゴに弾が当たった時の処理を呼び出す
        var titleLogo = new TitleLogoTriggerJob()
        {
            titleLogoSingletonLookup = _titleLogoSingletonLookup,
            dealDamageLookup = _dealDamageLookup,
            destroyableLookup = _destroyableLookup,
            audioPlayLookup = _audioPlayLookup,
            remainingPierceCountLookup = _remainingPierceCountLookup
        };

        // TitleLogoTriggerJobをスケジュール
        var titleLogoJobHandle = titleLogo.Schedule
        (
            SystemAPI.GetSingleton<SimulationSingleton>(),
            state.Dependency
        );

        // ジョブの依存関係を更新する
        state.Dependency = titleLogoJobHandle;

        // ジョブ完了を待機してから次の処理を実行する
        titleLogoJobHandle.Complete();

        // 次の画像に切り替える
        NextImage(ref state);
    }

    /// <summary>
    /// 次の画像に切り替える
    /// </summary>
    private void NextImage(ref SystemState state)
    {
        // TitleLogoSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<TitleLogoSingletonData>()) { return; }

        // シングルトンデータの取得
        var titleLogoSingleton = SystemAPI.GetSingleton<TitleLogoSingletonData>();

        // 次の画像に切り替える
        titleLogoSingleton.NextImage();

        // TitleLogoSingletonDataを持つEntityを取得
        Entity playerEntity = SystemAPI.GetSingletonEntity<TitleLogoSingletonData>();

        // isNextImageの値を実際のエンティティに反映
        state.EntityManager.SetComponentData(playerEntity, titleLogoSingleton);
    }
}

/// <summary>
/// タイトルロゴの衝突時の処理
/// </summary>
[BurstCompile]
public partial struct TitleLogoTriggerJob : ITriggerEventsJob
{
    public ComponentLookup<TitleLogoSingletonData> titleLogoSingletonLookup;
    public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
    public ComponentLookup<DestroyableData> destroyableLookup;
    public ComponentLookup<AudioPlayData> audioPlayLookup;
    public ComponentLookup<RemainingPierceCountData> remainingPierceCountLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        var entityA = triggerEvent.EntityA; // 接触対象
        var entityB = triggerEvent.EntityB; // isTriggerを有効にしている方

        // entityAがBulletIDealDamageDataを有しているか
        if (!dealDamageLookup.HasComponent(entityA)) { return; }

        // entityBがTitleLogoSingletonDataを有しているか
        if (!titleLogoSingletonLookup.HasComponent(entityB)) { return; }

        var titleLogoSingleton = titleLogoSingletonLookup[entityB];

        // ダメージ源がRemainingPierceCountDataを有していた
        if (remainingPierceCountLookup.HasComponent(entityA))
        {
            // 残り貫通回数をデクリメント
            var remainingPierceCount = remainingPierceCountLookup[entityA];
            remainingPierceCount.remainingPierceCount--;
            remainingPierceCountLookup[entityA] = remainingPierceCount;

            // ダメージ源がDestroyableDataを有していた
            if (destroyableLookup.HasComponent(entityA))
            {
                var destroyable = destroyableLookup[entityA];

                // 残り貫通回数が0以下か
                destroyable.isKilled = (remainingPierceCount.remainingPierceCount <= 0) ? true : false;

                // 変更を反映
                destroyableLookup[entityA] = destroyable;
            }
        }

        // EntityBがAudioPlayDataを有していた
        if (audioPlayLookup.HasComponent(entityB))
        {
            var audioPlay = audioPlayLookup[entityB];

            // タイトルロゴの被弾SEを再生
            audioPlay.AudioNumber = titleLogoSingleton.damageSENumber;
            audioPlayLookup[entityB] = audioPlay;
        }

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

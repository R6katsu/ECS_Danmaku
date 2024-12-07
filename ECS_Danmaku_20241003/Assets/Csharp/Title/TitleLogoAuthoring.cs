using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// タイトルロゴの情報（シングルトン前提）
/// </summary>
public struct TitleLogoSingletonData : IComponentData
{
    [Tooltip("何回の被弾で画像を切り替えるか")]
    public readonly int imageSwapCount;

    [Tooltip("タイトルロゴのダメージ効果音")]
    public readonly int damageSENumber;

    [Tooltip("現在の被弾回数")]
    public int currentImageSwapCount;

    [Tooltip("次の画像に切り替えるか")]
    public bool isNextImage;

    /// <summary>
    /// タイトルロゴのダメージ効果音
    /// </summary>
    /// <param name="imageSwapCount">何回で画像を切り替えるか</param>
    /// <param name="damageSENumber">タイトルロゴのダメージ効果音</param>
    public TitleLogoSingletonData(int imageSwapCount, int damageSENumber)
    {
        this.imageSwapCount = imageSwapCount;
        this.damageSENumber = damageSENumber;
        currentImageSwapCount = this.imageSwapCount;
        isNextImage = false;
    }

    /// <summary>
    /// 次の画像に切り替える
    /// </summary>
    public void NextImage()
    {
        // 次の画像に切り替えるフラグが立っていない
        if (!isNextImage) { return; }

        // 画像を切り替える
        var isTitleLogoBreak = TitleLogoSingleton.Instance.NextImage();

        // タイトルロゴの破壊が完了した
        if (isTitleLogoBreak != null && !(bool)isTitleLogoBreak)
        {
            // 破壊が完了した後の処理が登録されていたら実行する
            TitleLogoSingleton.Instance.breakAction?.Invoke();
        }

        isNextImage = false;
    }
}

/// <summary>
/// タイトルロゴの設定
/// </summary>
public class TitleLogoAuthoring : MonoBehaviour
{
    [SerializeField, Min(0), Header("何回で画像を切り替えるか")]
    private int _imageSwapCount = 0;

    [SerializeField, Min(0), Header("タイトルロゴのダメージ効果音")]
    private int _damageSENumber = 0;

    public class Baker : Baker<TitleLogoAuthoring>
    {
        public override void Bake(TitleLogoAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var titleLogoSingletonData = new TitleLogoSingletonData
                (
                    src._imageSwapCount,
                    src._damageSENumber
                );

            AddComponent(entity, titleLogoSingletonData);
        }
    }
}

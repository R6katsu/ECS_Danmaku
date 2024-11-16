using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// タイトルロゴのシングルトン
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class TitleLogoSingleton : SingletonMonoBehaviour<TitleLogoSingleton>
{
    [Tooltip("タイトルロゴが破壊可能な状態か")]
    private bool _isHFIOAHFIO = false;

    [Tooltip("現在の画像番号")]
    private int _currentImageNumber = 0;

    [Tooltip("破壊が完了した後の処理")]
    public Action breakAction = null;

    [Tooltip("前回の壊れかけのタイトルロゴ画像")]
    private SpriteRenderer _lastTitleLogoBreakImage = null;

    [Tooltip("タイトルロゴのAnimator")]
    private Animator _titleLogoAnimator = null;

    [SerializeField, Header("壊れかけのタイトルロゴ画像の配列")]
    private SpriteRenderer[] _titleLogoBreakImages = null;

    [SerializeField, Header("タイトルロゴの再生Trigger")]
    private string _titleLogoTriggerName = null;

    [SerializeField, Header("タイトルロゴ崩壊の再生Trigger")]
    private string _titleLogoBreakTriggerName = null;

    /// <summary>
    /// タイトルロゴのAnimator
    /// </summary>
    private Animator TitleLogoAnimator
    {
        get
        {
            if (_titleLogoAnimator == null)
            {
                // RequireComponent
                _titleLogoAnimator = GetComponent<Animator>();
            }

            return _titleLogoAnimator;
        }
    }

    /// <summary>
    /// 次の壊れかけのタイトルロゴ画像に切り替える
    /// </summary>
    /// <returns>次に切り替える画像が存在しない場合はfalse</returns>
    public bool? NextImage()
    {
        if (!_isHFIOAHFIO) { return null; }

        // あとでリファクタリングする
        GetComponent<SpriteRenderer>().enabled = false;

        // 既に配列の要素数を超過していた
        if (_titleLogoBreakImages.Length <= _currentImageNumber) { return null; }

        for (int i = 0; i < _titleLogoBreakImages.Length; i++)
        {
            if (i != _currentImageNumber) { continue; }

            // 次の画像に切り替える
            _titleLogoBreakImages[_currentImageNumber].gameObject.SetActive(true);

            // 前回の画像を無効化、前回の画像を更新
            _lastTitleLogoBreakImage?.gameObject.SetActive(false);
            _lastTitleLogoBreakImage = _titleLogoBreakImages[_currentImageNumber];
        }

        // 画像の番号をインクリメント
        _currentImageNumber++;

        // 配列の要素数を超過した
        if (_titleLogoBreakImages.Length <= _currentImageNumber) { return false; }

        return true;
    }

    /// <summary>
    /// タイトルロゴを再生
    /// </summary>
    /// <returns>null</returns>
    public IEnumerator TitleLogoAnimation()
    {
        // あとでリファクタリングする
        GetComponent<SpriteRenderer>().enabled = true;

        // タイトルロゴのアニメーションを再生
        yield return UIAnimationAndEndAction.Instance.AnimationAndEndAction
            (
                TitleLogoAnimator,
                _titleLogoTriggerName
            );

        _isHFIOAHFIO = true;
    }

    /// <summary>
    /// タイトルロゴを破壊して破片にする
    /// </summary>
    /// <returns>null</returns>
    public IEnumerator TitleLogoBreakAnimation()
    {
        _isHFIOAHFIO = false;

        // あとでリファクタリングする
        GetComponent<SpriteRenderer>().enabled = true;

        // 画像を全て無効化
        for (int i = 0; i < _titleLogoBreakImages.Length; i++)
        {
            _titleLogoBreakImages[i].gameObject.SetActive(false);
        }

        // タイトルロゴの破壊アニメーションを再生
        yield return UIAnimationAndEndAction.Instance.AnimationAndEndAction
            (
                TitleLogoAnimator,
                _titleLogoBreakTriggerName
            );
    }
}

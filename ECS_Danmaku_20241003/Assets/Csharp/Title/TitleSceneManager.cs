using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEngine.Rendering;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// タイトルシーン
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class TitleSceneManager : SingletonMonoBehaviour<TitleSceneManager>
{
    /// <summary>
    /// タイトルシーンの状態
    /// </summary>
    private enum TitleSceneState
    {
        [Tooltip("初期値")] Loading,
        [Tooltip("初期化")] Start,
        [Tooltip("タイトルロゴ")] TitleLogo,
        [Tooltip("タイトル画面")] Title,
        [Tooltip("タイトル画面終了")] TitleClose
    }

    [Tooltip("タイトルロゴが破壊可能な状態か")]
    private bool _isTitleLogoDestructible = false;

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

    [SerializeField, Min(0), Header("タイトルBGMの番号")]
    private int _titleBGMNumber = 0;

    [SerializeField, Header("シーン遷移のトランジションの名称")]
    private TransitionName _transitionName = 0;

    [Tooltip("タイトルシーンの状態")]
    private TitleSceneState _titleSceneState = 0;

    /// <summary>
    /// タイトルロゴの当たり判定が有効か否か
    /// </summary>
    public bool HasTitleLogoCollision { get; set; }

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
    /// タイトルシーンの状態
    /// </summary>
    private TitleSceneState MyTitleSceneState
    {
        set
        {
            // 前回から変更があった
            if (_titleSceneState != value)
            {
                _titleSceneState = value;

                // 変更があったTitleSceneStateに対応する処理を実行
                ChangeTitleSceneState();
            }
        }
    }

    private void Start()
    {
        MyTitleSceneState = TitleSceneState.Start;
    }

    /// <summary>
    /// TitleSceneStateに対応する処理を実行
    /// </summary>
    private void ChangeTitleSceneState()
    {
        switch (_titleSceneState)
        {
            case TitleSceneState.Start:
                // タイトルロゴの当たり判定を無効化
                HasTitleLogoCollision = false;

                Cursor.lockState = CursorLockMode.Locked;   // カーソルを画面中央にロックする
                Cursor.visible = false;                     // カーソル非表示

                // タイトルロゴが壊れた時の処理を登録
                breakAction = () =>
                {
                    MyTitleSceneState = TitleSceneState.TitleClose;
                };

                // タイトルロゴの処理に移行
                MyTitleSceneState = TitleSceneState.TitleLogo;
                break;

            case TitleSceneState.TitleLogo:
                // タイトル画面のBGM再生
                AudioPlayManager.Instance.PlayBGM(_titleBGMNumber);

                // タイトルロゴの再生と終了までの待機
                StartCoroutine(TitleLogoAnimationAndWait());
                break;

            case TitleSceneState.Title:
                // タイトルロゴの当たり判定を有効化
                HasTitleLogoCollision = true;
                break;

            case TitleSceneState.TitleClose:
                // タイトルロゴの当たり判定を無効化
                HasTitleLogoCollision = false;

                // タイトル画面を閉じ、新しいシーンを読み込む
                StartCoroutine(TitleClose());
                break;

            default:
            case TitleSceneState.Loading:
                break;
        }
    }

    /// <summary>
    /// タイトルロゴの再生と終了までの待機
    /// </summary>
    /// <returns></returns>
    private IEnumerator TitleLogoAnimationAndWait()
    {
        // タイトルロゴを再生
        yield return StartCoroutine(TitleLogoAnimation());

        // タイトル画面の処理を開始
        MyTitleSceneState = TitleSceneState.Title;
    }

    /// <summary>
    /// タイトル画面を閉じ、新しいシーンを読み込む
    /// </summary>
    /// <returns></returns>
    private IEnumerator TitleClose()
    {
        // タイトルロゴを壊す
        yield return StartCoroutine(TitleLogoBreakAnimation());

        var transition = TransitionDirector.Instance.GetTransition(_transitionName);

        // 現在のシーンの番号を取得
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 現在のシーン番号をインクリメント
        currentSceneIndex++;

        transition.StartTransition(currentSceneIndex);
    }

    /// <summary>
    /// 次の壊れかけのタイトルロゴ画像に切り替える
    /// </summary>
    /// <returns>次に切り替える画像が存在しない場合はfalse</returns>
    public bool? NextImage()
    {
        if (!_isTitleLogoDestructible) { return null; }

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
    /// <returns></returns>
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

        _isTitleLogoDestructible = true;
    }

    /// <summary>
    /// タイトルロゴを破壊して破片にする
    /// </summary>
    /// <returns></returns>
    public IEnumerator TitleLogoBreakAnimation()
    {
        _isTitleLogoDestructible = false;

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

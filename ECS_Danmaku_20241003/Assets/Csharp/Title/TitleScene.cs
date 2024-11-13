using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


#if UNITY_EDITOR
using UnityEngine.Rendering;
using System.Collections.Generic;
#endif

/// <summary>
/// タイトルシーン
/// </summary>
public class TitleScene : MonoBehaviour
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

    [SerializeField, Min(0), Header("タイトルBGMの番号")]
    private int _titleBGMNumber = 0;

    [SerializeField, Header("タイトルロゴのAnimator")]
    private Animator _titleLogoAnimator = null;

    [SerializeField, Header("シーン遷移のトランジション")]
    private Transform _transitionTransform = null;

    [SerializeField, Header("タイトルロゴの再生Trigger")]
    private string _titleLogoTriggerName = null;

    [Tooltip("シーン遷移のトランジション")]
    private ITransition _transition = null;

    [Tooltip("タイトルシーンの状態")]
    private TitleSceneState _titleSceneState = 0;

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

    private void ChangeTitleSceneState()
    {
        switch (_titleSceneState)
        {
            case TitleSceneState.Start:
                if (!_transitionTransform.TryGetComponent(out _transition))
                {
#if UNITY_EDITOR
                    Debug.LogError("_transitionTransformがITransitionを有していない");
#endif
                    enabled = false;
                    return;
                }

                MyTitleSceneState = TitleSceneState.TitleLogo;
                break;

            case TitleSceneState.TitleLogo:
                // タイトル画面のBGM再生
                AudioManager.Instance.PlayBGM(_titleBGMNumber);

                // タイトルロゴの再生と終了までの待機
                StartCoroutine(TitleLogoAnimationAndWait());
                break;

            case TitleSceneState.Title:
                // タイトル画面の処理を開始
                StartCoroutine(TitleUpdate());
                break;

            case TitleSceneState.TitleClose:
                // タイトル画面を閉じ、新しいシーンを読み込む
                TitleClose();
                break;

            default:
            case TitleSceneState.Loading:
                break;
        }
    }

    /// <summary>
    /// タイトルロゴの再生と終了までの待機
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator TitleLogoAnimationAndWait()
    {
        // タイトルロゴを再生
        _titleLogoAnimator.SetTrigger(_titleLogoTriggerName);

        // アニメーションの遷移を待つ
        yield return null;

        // アニメーション情報の取得
        AnimatorStateInfo stateInfo = _titleLogoAnimator.GetCurrentAnimatorStateInfo(0);

        // 正しい長さが取得できるまで待機
        while (stateInfo.length == 1)
        {
            yield return null;
            stateInfo = _titleLogoAnimator.GetCurrentAnimatorStateInfo(0);
        }

        // アニメーション終了まで待機
        float animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength);

        // タイトル画面の処理を開始
        MyTitleSceneState = TitleSceneState.Title;
    }

    /// <summary>
    /// タイトル画面の処理
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator TitleUpdate()
    {
        while (_titleSceneState == TitleSceneState.Title)
        {
            yield return null;

            // 入力がなかった
            if (!Input.anyKeyDown) { continue; }

            // タイトル画面の処理を終了
            MyTitleSceneState = TitleSceneState.TitleClose;            
        }
    }

    /// <summary>
    /// タイトル画面を閉じ、新しいシーンを読み込む
    /// </summary>
    private void TitleClose()
    {
        _transition.StartTransition();
    }
}

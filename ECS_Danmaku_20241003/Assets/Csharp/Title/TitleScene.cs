using UnityEngine;
using System.Collections;


#if UNITY_EDITOR
using UnityEngine.SceneManagement;
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

    [SerializeField, Header("シーン遷移のトランジションの名称")]
    private TransitionName _transitionName = 0;

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
                // カーソルを画面中央にロックする
                Cursor.lockState = CursorLockMode.Locked;

                // カーソル非表示
                Cursor.visible = false;

                TitleLogoSingleton.Instance.breakAction = () =>
                {
                    Debug.Log("TitleLogoSingleton.Instance.breakAction");
                    MyTitleSceneState = TitleSceneState.TitleClose;
                };

                MyTitleSceneState = TitleSceneState.TitleLogo;
                break;

            case TitleSceneState.TitleLogo:
                // タイトル画面のBGM再生
                AudioPlayManager.Instance.PlayBGM(_titleBGMNumber);

                // タイトルロゴの再生と終了までの待機
                StartCoroutine(TitleLogoAnimationAndWait());
                break;

            case TitleSceneState.Title:
                // タイトル画面の処理を開始
                StartCoroutine(TitleUpdate());
                break;

            case TitleSceneState.TitleClose:
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
    /// <returns>null</returns>
    private IEnumerator TitleLogoAnimationAndWait()
    {
        // タイトルロゴを再生
        yield return StartCoroutine(TitleLogoSingleton.Instance.TitleLogoAnimation());

        // タイトル画面の処理を開始
        MyTitleSceneState = TitleSceneState.Title;
    }

    /// <summary>
    /// タイトル画面の処理
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator TitleUpdate()
    {
        yield return null;

        Debug.Log("ここから続き");

        // PLを登場させ、射撃できるようにする
        // タイトルロゴに攻撃できるようにする
        // WASDやShiftの操作説明を出現させる
        // タイトルロゴを攻撃した時、壊せ！という文字が表示されるようにする

        /*
        while (_titleSceneState == TitleSceneState.Title)
        {
            yield return null;

            // 入力がなかった
            if (!Input.anyKeyDown) { continue; }

            // タイトル画面の処理を終了
            MyTitleSceneState = TitleSceneState.TitleClose;            
        }
        */
    }

    /// <summary>
    /// タイトル画面を閉じ、新しいシーンを読み込む
    /// </summary>
    private IEnumerator TitleClose()
    {
        Debug.Log("TitleClose");

        // タイトルロゴを壊す
        yield return StartCoroutine(TitleLogoSingleton.Instance.TitleLogoBreakAnimation());

        var transition = TransitionDirector.Instance.GetTransition(_transitionName);

        // 現在のシーンの番号を取得
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 現在のシーン番号をインクリメント
        currentSceneIndex++;

        transition.StartTransition(currentSceneIndex);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
#endif

/// <summary>
/// 
/// </summary>
public class GameSceneUIDirector : SingletonMonoBehaviour<GameSceneUIDirector>
{
    [SerializeField, Header("UI操作の配列")]
    private UIController[] _activatableUIControllerElements = null;

    public void Retry()
    {
        // 現在のシーンのインデックスを取得
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 現在のシーンを読み込む
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void GameEnd()
    {
#if UNITY_EDITOR
        // プレイモードを終了
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // ビルドされたゲームを終了
            Application.Quit();
#endif
    }

    // GameEndのUIControllerを有効にする
    // また、UIControllerの現在選択中の要素番号(x,y)を保持する
    // GameEndはゲームクリアとゲームオーバーの両方に使い回す

    /// <summary>
    /// 単一のUI要素を有効化する。<br/>
    /// 使い方：関数を実行する側でenumを実装、intに変換してください。
    /// </summary>
    /// <param name="elementNumber">有効化するUI要素の番号</param>
    public void ActivateSingleUIControllerElement(int elementNumber)
    {
        // 配列の範囲外だった
        if (elementNumber < 0 || elementNumber >= _activatableUIControllerElements.Length)
        {
#if UNITY_EDITOR
            Debug.LogError($"elementNumber: {elementNumber} が範囲外！");
#endif
            return;
        }

        // 全てのUIを無効化
        DisableAllUIController();

        // 対応するUI要素を有効化
        _activatableUIControllerElements[elementNumber].gameObject.SetActive(true);



        // 選択された要素番号に登録されたイベントを実行する必要がある
        // gameObject.SetActive(true)　ではない。Invokeをする？
        // Enterで実行なども実装する
        // UIの操作が有効になった場合、決定ボタンを押した時にこの関数を呼び出す処理を追加する？
    }

    /// <summary>
    /// 全てのUIを無効化
    /// </summary>
    public void DisableAllUIController()
    {
        // 全てのUIを無効化
        foreach (var uiElement in _activatableUIControllerElements)
        {
            uiElement.gameObject.SetActive(false);
        }
    }
}

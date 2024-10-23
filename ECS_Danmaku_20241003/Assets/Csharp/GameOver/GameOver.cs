using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// ゲームオーバー
/// </summary>
public class GameOver : SingletonMonoBehaviour<GameOver>
{
    [SerializeField, Header("表示するテキスト")]
    private TextMeshProUGUI _gameOverUGUI = null;

    private bool _isGameOver = false;

    override protected void Awake()
    {
        base.Awake();

        _isGameOver = false;
        _gameOverUGUI.enabled = false;
    }

    /// <summary>
    /// ゲームオーバーのフラグを有効化
    /// </summary>
    public void OnGameOver() => _isGameOver = true;

    private void Update()
    {
        if (!_isGameOver) { return; }

        // テキストが無効だったら有効にする
        if  (!_gameOverUGUI.enabled)
        { _gameOverUGUI.enabled = true; }

        // Enterが押された
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _gameOverUGUI.enabled = false;
            _isGameOver = false;

            // 現在のシーンのインデックスを取得
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // 現在のシーンを読み込む
            SceneManager.LoadScene(currentSceneIndex);
        }

        // Escapeが押された
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _gameOverUGUI.enabled = false;
            _isGameOver = false;

#if UNITY_EDITOR
            // プレイモードを終了
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // ビルドされたゲームを終了
            Application.Quit();
#endif
        }
    }
}

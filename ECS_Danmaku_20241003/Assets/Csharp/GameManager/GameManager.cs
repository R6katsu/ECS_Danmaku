using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// ゲーム管理
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    // ゲームが開始するまではジングルを再生
    // PLを動かせるようになったらBGMを再生する

    /// <summary>
    /// ゲームの状態
    /// </summary>
    public enum GameState
    { 
        Loading,
        Start,
        Game,
        GameClear,
        GameOver,
        End
    }

    /// <summary>
    /// UIの名称
    /// </summary>
    private enum UIName : int
    {
        GameClearUI,
        GameOverUI
    }

    [SerializeField, Header("経過時間のテキスト")]
    private TextMeshProUGUI _elapsedTimeUGUI = null;

    private GameState _gameState = 0;

    [Tooltip("経過時間")]
    private float _elapsedTime = 0.0f;

    [Tooltip("現在時刻")]
    private int _currentTime = 0;

    /// <summary>
    /// ゲームの状態
    /// </summary>
    public GameState MyGameState
    {
        get => _gameState;
        set
        {
            if (_gameState != value)
            {
                _gameState = value;

                // 変化があった
                ChangeGameState();
            }
        }
    }

    private void Start()
    {
        MyGameState = GameState.Start;
    }

    /// <summary>
    /// ゲームの状態に対応した処理
    /// </summary>
    public void ChangeGameState()
    {
        switch (MyGameState)
        {
             case GameState.Start:
                // BGM開始
                AudioPlayManager.Instance.PlayBGM(0);

                // Gameに移行
                MyGameState = GameState.Game;
                break;

            case GameState.Game:
                StartCoroutine(Game());
                break;

            case GameState.GameClear:
                // アニメーションが終わってからボタンを表示する
                // アニメーション側でボタンを表示するタイミングを考えないといけない

                ActivatableUIDirector.Instance.ActivateSingleUIElement((int)UIName.GameClearUI);

                MainThreadExecutor.Instance.Enqueue(() =>
                {
                    AudioPlayManager.Instance.PauseBGM();
                });
                break;

            case GameState.GameOver:
                ActivatableUIDirector.Instance.ActivateSingleUIElement((int)UIName.GameOverUI);

                MainThreadExecutor.Instance.Enqueue (() =>
                {
                    AudioPlayManager.Instance.PauseBGM();
                });
                break;

            case GameState.End:
                break;

            case GameState.Loading:
            default:
                break;
        }
    }

    /// <summary>
    /// GameState.Gameの処理
    /// </summary>
    /// <returns>null</returns>
    public IEnumerator Game()
    {
        // GameStateがGameの間はループする
        while (MyGameState == GameState.Game)
        {
            yield return null;

            _elapsedTime += Time.deltaTime;

            if (_currentTime != (int)_elapsedTime)
            {
                _currentTime = (int)_elapsedTime;
                _elapsedTimeUGUI.text = _currentTime.ToString();
            }
        }
    }
}

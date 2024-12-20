using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using TMPro;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
#endif

// リファクタリング済み

/// <summary>
/// ゲームの状態
/// </summary>
public enum GameState : byte
{
    [Tooltip("初期値")] Loading,
    [Tooltip("ゲーム開始")] Start,
    [Tooltip("ゲーム中")] Game,
    [Tooltip("ゲームクリア")] GameClear,
    [Tooltip("ゲームオーバー")] GameOver
}

/// <summary>
/// ゲームの進行を管理
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    /// <summary>
    /// UIの種類
    /// </summary>
    private enum UIType : int
    {
        GameClearUI,
        GameOverUI
    }

    [SerializeField, Min(0), Header("BGMの番号")]
    private int _bgmNumber = 0;

    private GameState _gameState = 0;

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
                // 既にGameClearだったら変更しない
                _gameState = (_gameState == GameState.GameClear) ? _gameState : value;

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
                AudioPlayManager.Instance.PlayBGM(_bgmNumber);

                // Gameに移行
                MyGameState = GameState.Game;
                break;

            case GameState.Game:
                break;

            case GameState.GameClear:
                ActivatableUIDirector.Instance.ActivateSingleUIElement((int)UIType.GameClearUI);

                MainThreadExecutor.Instance.Enqueue(() =>
                {
                    AudioPlayManager.Instance.PauseBGM();
                });
                break;

            case GameState.GameOver:
                ActivatableUIDirector.Instance.ActivateSingleUIElement((int)UIType.GameOverUI);

                MainThreadExecutor.Instance.Enqueue (() =>
                {
                    AudioPlayManager.Instance.PauseBGM();
                });
                break;

            case GameState.Loading:
            default:
                break;
        }
    }
}

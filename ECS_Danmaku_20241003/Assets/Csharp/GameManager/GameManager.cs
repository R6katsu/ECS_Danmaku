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

    [SerializeField, Header("ゲームカメラ")]
    private Camera _gameCamera = null;

    private GameState _gameState = 0;

    [Tooltip("経過時間")]
    private float _elapsedTime = 0.0f;

    [Tooltip("現在時刻")]
    private int _currentTime = 0;

    [Tooltip("ゲームカメラの位置")]
    private Vector3 _gameCameraPosition = Vector3.zero;

    [Tooltip("基準点からズレたカメラの位置")]
    private Vector3 _cameraOffset = Vector3.zero;

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

    public float3 GameCameraPosition
    {
        set
        {
            if (_gameCameraPosition != (Vector3)value)
            {
                _gameCameraPosition = value;

                // 移動をカメラ位置に反映
                _gameCameraPosition.z = 0;
                _gameCamera.transform.position = _gameCameraPosition + _cameraOffset;
            }
        }
    }

    private void OnEnable()
    {
        _cameraOffset = _gameCamera.transform.position;
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
                AudioManager.Instance.PlayBGM(0);

                // Gameに移行
                MyGameState = GameState.Game;
                break;

            case GameState.Game:
                //OnEnemySpawnerSystem();

                StartCoroutine(Game());
                break;

            case GameState.GameClear:
                ActivatableUIDirector.Instance.ActivateSingleUIElement((int)UIName.GameClearUI);

                GameSceneUIDirector.Instance.ActivateSingleUIControllerElement(0);
                break;

            case GameState.GameOver:
                ActivatableUIDirector.Instance.ActivateSingleUIElement((int)UIName.GameOverUI);

                GameSceneUIDirector.Instance.ActivateSingleUIControllerElement(0);
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

    public void OnEnemySpawnerSystem()
    {
        // World取得
        World defaultWorld = World.DefaultGameObjectInjectionWorld;

        // SystemHandleを取得
        var systemHandle = defaultWorld.GetExistingSystem<EnemySpawnerSystem>();

        // 実際のSystemの参照を取得する
        var enemySpawnerSystem = defaultWorld.Unmanaged.GetUnsafeSystemRef<EnemySpawnerSystem>(systemHandle);

        // EnemySpawnerSystemを有効にする
        enemySpawnerSystem.isSelfEnable = true;
    }

    public void OffEnemySpawnerSystem()
    {
        // World取得
        World defaultWorld = World.DefaultGameObjectInjectionWorld;

        // SystemHandleを取得
        var systemHandle = defaultWorld.GetExistingSystem<EnemySpawnerSystem>();

        // 実際のSystemの参照を取得する
        var enemySpawnerSystem = defaultWorld.Unmanaged.GetUnsafeSystemRef<EnemySpawnerSystem>(systemHandle);

        // EnemySpawnerSystemを有効にする
        enemySpawnerSystem.isSelfEnable = false;
    }
}

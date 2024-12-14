using System.Collections;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �Q�[���̏��
/// </summary>
public enum GameState : byte
{
    [Tooltip("�����l")] Loading,
    [Tooltip("�Q�[���J�n")] Start,
    [Tooltip("�Q�[����")] Game,
    [Tooltip("�Q�[���N���A")] GameClear,
    [Tooltip("�Q�[���I�[�o�[")] GameOver
}

/// <summary>
/// �Q�[���̐i�s���Ǘ�
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    /// <summary>
    /// UI�̖���
    /// </summary>
    private enum UIName : int
    {
        GameClearUI,
        GameOverUI
    }

    [SerializeField, Header("�o�ߎ��Ԃ̃e�L�X�g")]
    private TextMeshProUGUI _elapsedTimeUGUI = null;

    [SerializeField, Min(0), Header("BGM�̔ԍ�")]
    private int _bgmNumber = 0;

    private GameState _gameState = 0;

    [Tooltip("�o�ߎ���")]
    private float _elapsedTime = 0.0f;

    [Tooltip("���ݎ���")]
    private int _currentTime = 0;

    /// <summary>
    /// �Q�[���̏��
    /// </summary>
    public GameState MyGameState
    {
        get => _gameState;
        set
        {
            if (_gameState != value)
            {
                // ����GameClear��������ύX���Ȃ�
                _gameState = (_gameState == GameState.GameClear) ? _gameState : value;

                // �ω���������
                ChangeGameState();
            }
        }
    }

    private void Start()
    {
        MyGameState = GameState.Start;
    }

    /// <summary>
    /// �Q�[���̏�ԂɑΉ���������
    /// </summary>
    public void ChangeGameState()
    {
        switch (MyGameState)
        {
             case GameState.Start:
                // BGM�J�n
                AudioPlayManager.Instance.PlayBGM(_bgmNumber);

                // Game�Ɉڍs
                MyGameState = GameState.Game;
                break;

            case GameState.Game:
                StartCoroutine(Game());
                break;

            case GameState.GameClear:
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

            case GameState.Loading:
            default:
                break;
        }
    }

    /// <summary>
    /// GameState.Game�̏���
    /// </summary>
    /// <returns>null</returns>
    public IEnumerator Game()
    {
        // GameState��Game�̊Ԃ̓��[�v����
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

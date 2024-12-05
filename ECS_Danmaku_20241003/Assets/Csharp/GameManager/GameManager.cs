using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// �Q�[���Ǘ�
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    // �Q�[�����J�n����܂ł̓W���O�����Đ�
    // PL�𓮂�����悤�ɂȂ�����BGM���Đ�����

    /// <summary>
    /// �Q�[���̏��
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
    /// UI�̖���
    /// </summary>
    private enum UIName : int
    {
        GameClearUI,
        GameOverUI
    }

    [SerializeField, Header("�o�ߎ��Ԃ̃e�L�X�g")]
    private TextMeshProUGUI _elapsedTimeUGUI = null;

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
                _gameState = value;

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
                AudioPlayManager.Instance.PlayBGM(0);

                // Game�Ɉڍs
                MyGameState = GameState.Game;
                break;

            case GameState.Game:
                StartCoroutine(Game());
                break;

            case GameState.GameClear:
                // �A�j���[�V�������I����Ă���{�^����\������
                // �A�j���[�V�������Ń{�^����\������^�C�~���O���l���Ȃ��Ƃ����Ȃ�

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

using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    // PL�̈ړ��͈͂���ʓ��Ɏ��߂�
    // �J�������΂ߌ��̉�p�ɂ���B���̏ꍇ�A�G�̏o���ʒu���ی����ɂȂ�B������o�Ă���悤�ɂ���H
    // ����A����͕\����������Ȃ̂ł�߂�B���ʂ���ł����B

    /// <summary>
    /// �Q�[���̏��
    /// </summary>
    public enum GameState
    { 
        Loading,
        Start,
        Game,
        End
    }

    [SerializeField, Header("�o�ߎ��Ԃ̃e�L�X�g")]
    private TextMeshProUGUI _elapsedTimeUGUI = null;

    [SerializeField, Header("�Q�[���J����")]
    private Camera _gameCamera = null;

    private GameState _gameState = 0;

    [Tooltip("�o�ߎ���")]
    private float _elapsedTime = 0.0f;

    [Tooltip("���ݎ���")]
    private int _currentTime = 0;

    [Tooltip("�Q�[���J�����̈ʒu")]
    private Vector3 _gameCameraPosition = Vector3.zero;

    [Tooltip("��_����Y�����J�����̈ʒu")]
    private Vector3 _cameraOffset = Vector3.zero;

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

    public float3 GameCameraPosition
    {
        set
        {
            if (_gameCameraPosition != (Vector3)value)
            {
                _gameCameraPosition = value;

                // �ړ����J�����ʒu�ɔ��f
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
    /// �Q�[���̏�ԂɑΉ���������
    /// </summary>
    public void ChangeGameState()
    {
        switch (MyGameState)
        {
            case GameState.Loading:
                // None�B�����l
                break;

            case GameState.Start:
                // BGM�J�n
                AudioManager.Instance.PlayBGM(0);

                // Game�Ɉڍs
                MyGameState = GameState.Game;
                break;

            case GameState.Game:
                StartCoroutine(Game());
                break;

            case GameState.End:
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

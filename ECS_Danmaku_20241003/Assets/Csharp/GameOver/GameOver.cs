using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// �Q�[���I�[�o�[
/// </summary>
public class GameOver : SingletonMonoBehaviour<GameOver>
{
    [SerializeField, Header("�\������UI")]
    private Transform _gameOverUI = null;

    private bool _isGameOver = false;

    override protected void Awake()
    {
        base.Awake();

        _isGameOver = false;
        _gameOverUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// �Q�[���I�[�o�[�̃t���O��L����
    /// </summary>
    public void OnGameOver() => _isGameOver = true;

    private void Update()
    {
        if (!_isGameOver) { return; }

        // �e�L�X�g��������������L���ɂ���
        if (!_gameOverUI.gameObject.activeSelf)
        { _gameOverUI.gameObject.SetActive(true); }

        // Enter�������ꂽ
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _gameOverUI.gameObject.SetActive(false);
            _isGameOver = false;

            // ���݂̃V�[���̃C���f�b�N�X���擾
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // ���݂̃V�[����ǂݍ���
            SceneManager.LoadScene(currentSceneIndex);
        }

        // Escape�������ꂽ
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _gameOverUI.gameObject.SetActive(false);
            _isGameOver = false;

#if UNITY_EDITOR
            // �v���C���[�h���I��
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // �r���h���ꂽ�Q�[�����I��
            Application.Quit();
#endif
        }
    }
}

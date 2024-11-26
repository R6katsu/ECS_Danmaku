using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
#endif

/// <summary>
/// 
/// </summary>
public class GameSceneUIDirector : SingletonMonoBehaviour<GameSceneUIDirector>
{
    [SerializeField, Header("UI����̔z��")]
    private UIController[] _activatableUIControllerElements = null;

    public void Retry()
    {
        // ���݂̃V�[���̃C���f�b�N�X���擾
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // ���݂̃V�[����ǂݍ���
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void GameEnd()
    {
#if UNITY_EDITOR
        // �v���C���[�h���I��
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // �r���h���ꂽ�Q�[�����I��
            Application.Quit();
#endif
    }

    /// <summary>
    /// �P���UI�v�f��L��������B<br/>
    /// �g�����F�֐������s���鑤��enum�������Aint�ɕϊ����Ă��������B
    /// </summary>
    /// <param name="elementNumber">�L��������UI�v�f�̔ԍ�</param>
    public void ActivateSingleUIControllerElement(int elementNumber)
    {
        // �z��͈̔͊O������
        if (elementNumber < 0 || elementNumber >= _activatableUIControllerElements.Length)
        {
#if UNITY_EDITOR
            Debug.LogError($"elementNumber: {elementNumber} ���͈͊O�I");
#endif
            return;
        }

        var uiController = _activatableUIControllerElements[elementNumber];

        uiController.Enable();
    }
}

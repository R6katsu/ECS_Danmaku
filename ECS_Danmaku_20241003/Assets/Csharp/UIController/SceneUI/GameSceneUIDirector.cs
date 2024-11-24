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

    // GameEnd��UIController��L���ɂ���
    // �܂��AUIController�̌��ݑI�𒆂̗v�f�ԍ�(x,y)��ێ�����
    // GameEnd�̓Q�[���N���A�ƃQ�[���I�[�o�[�̗����Ɏg����

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

        // �S�Ă�UI�𖳌���
        DisableAllUIController();

        // �Ή�����UI�v�f��L����
        _activatableUIControllerElements[elementNumber].gameObject.SetActive(true);



        // �I�����ꂽ�v�f�ԍ��ɓo�^���ꂽ�C�x���g�����s����K�v������
        // gameObject.SetActive(true)�@�ł͂Ȃ��BInvoke������H
        // Enter�Ŏ��s�Ȃǂ���������
        // UI�̑��삪�L���ɂȂ����ꍇ�A����{�^�������������ɂ��̊֐����Ăяo��������ǉ�����H
    }

    /// <summary>
    /// �S�Ă�UI�𖳌���
    /// </summary>
    public void DisableAllUIController()
    {
        // �S�Ă�UI�𖳌���
        foreach (var uiElement in _activatableUIControllerElements)
        {
            uiElement.gameObject.SetActive(false);
        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEngine.Rendering;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �^�C�g���V�[��
/// </summary>
public class TitleScene : MonoBehaviour
{
    /// <summary>
    /// �^�C�g���V�[���̏��
    /// </summary>
    private enum TitleSceneState
    {
        [Tooltip("�����l")] Loading,
        [Tooltip("������")] Start,
        [Tooltip("�^�C�g�����S")] TitleLogo,
        [Tooltip("�^�C�g�����")] Title,
        [Tooltip("�^�C�g����ʏI��")] TitleClose
    }

    [SerializeField, Min(0), Header("�^�C�g��BGM�̔ԍ�")]
    private int _titleBGMNumber = 0;

    [SerializeField, Header("�V�[���J�ڂ̃g�����W�V�����̖���")]
    private TransitionName _transitionName = 0;

    [Tooltip("�^�C�g���V�[���̏��")]
    private TitleSceneState _titleSceneState = 0;

    /// <summary>
    /// �^�C�g���V�[���̏��
    /// </summary>
    private TitleSceneState MyTitleSceneState
    {
        set
        {
            // �O�񂩂�ύX��������
            if (_titleSceneState != value)
            {
                _titleSceneState = value;

                // �ύX��������TitleSceneState�ɑΉ����鏈�������s
                ChangeTitleSceneState();
            }
        }
    }

    private void Start()
    {
        MyTitleSceneState = TitleSceneState.Start;
    }

    /// <summary>
    /// TitleSceneState�ɑΉ����鏈�������s
    /// </summary>
    private void ChangeTitleSceneState()
    {
        switch (_titleSceneState)
        {
            case TitleSceneState.Start:
                Cursor.lockState = CursorLockMode.Locked;   // �J�[�\������ʒ����Ƀ��b�N����
                Cursor.visible = false;                     // �J�[�\����\��

                // �^�C�g�����S����ꂽ���̏�����o�^
                TitleLogoSingleton.Instance.breakAction = () =>
                {
                    MyTitleSceneState = TitleSceneState.TitleClose;
                };

                // �^�C�g�����S�̏����Ɉڍs
                MyTitleSceneState = TitleSceneState.TitleLogo;
                break;

            case TitleSceneState.TitleLogo:
                // �^�C�g����ʂ�BGM�Đ�
                AudioPlayManager.Instance.PlayBGM(_titleBGMNumber);

                // �^�C�g�����S�̍Đ��ƏI���܂ł̑ҋ@
                StartCoroutine(TitleLogoAnimationAndWait());
                break;

            case TitleSceneState.Title:
                // �^�C�g����ʂ̏������J�n
                break;

            case TitleSceneState.TitleClose:
                // �^�C�g����ʂ���A�V�����V�[����ǂݍ���
                StartCoroutine(TitleClose());
                break;

            default:
            case TitleSceneState.Loading:
                break;
        }
    }

    /// <summary>
    /// �^�C�g�����S�̍Đ��ƏI���܂ł̑ҋ@
    /// </summary>
    /// <returns></returns>
    private IEnumerator TitleLogoAnimationAndWait()
    {
        // �^�C�g�����S���Đ�
        yield return StartCoroutine(TitleLogoSingleton.Instance.TitleLogoAnimation());

        // �^�C�g����ʂ̏������J�n
        MyTitleSceneState = TitleSceneState.Title;
    }

    /// <summary>
    /// �^�C�g����ʂ���A�V�����V�[����ǂݍ���
    /// </summary>
    /// <returns></returns>
    private IEnumerator TitleClose()
    {
        // �^�C�g�����S����
        yield return StartCoroutine(TitleLogoSingleton.Instance.TitleLogoBreakAnimation());

        var transition = TransitionDirector.Instance.GetTransition(_transitionName);

        // ���݂̃V�[���̔ԍ����擾
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // ���݂̃V�[���ԍ����C���N�������g
        currentSceneIndex++;

        transition.StartTransition(currentSceneIndex);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


#if UNITY_EDITOR
using UnityEngine.Rendering;
using System.Collections.Generic;
#endif

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

    [SerializeField, Header("�V�[���J�ڂ̃g�����W�V����")]
    private Transform _transitionTransform = null;

    [Tooltip("�V�[���J�ڂ̃g�����W�V����")]
    private ITransition _transition = null;

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

    private void ChangeTitleSceneState()
    {
        switch (_titleSceneState)
        {
            case TitleSceneState.Start:
                if (!_transitionTransform.TryGetComponent(out _transition))
                {
#if UNITY_EDITOR
                    Debug.LogError("_transitionTransform��ITransition��L���Ă��Ȃ�");
#endif
                    enabled = false;
                    return;
                }

                TitleLogoSingleton.Instance.breakAction = () =>
                {
                    Debug.Log("TitleLogoSingleton.Instance.breakAction");
                    MyTitleSceneState = TitleSceneState.TitleClose;
                };

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
                StartCoroutine(TitleUpdate());
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
    /// <returns>null</returns>
    private IEnumerator TitleLogoAnimationAndWait()
    {
        // �^�C�g�����S���Đ�
        yield return StartCoroutine(TitleLogoSingleton.Instance.TitleLogoAnimation());

        // �^�C�g����ʂ̏������J�n
        MyTitleSceneState = TitleSceneState.Title;
    }

    /// <summary>
    /// �^�C�g����ʂ̏���
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator TitleUpdate()
    {
        yield return null;

        Debug.Log("�������瑱��");

        // PL��o�ꂳ���A�ˌ��ł���悤�ɂ���
        // �^�C�g�����S�ɍU���ł���悤�ɂ���
        // WASD��Shift�̑���������o��������
        // �^�C�g�����S���U���������A�󂹁I�Ƃ����������\�������悤�ɂ���

        /*
        while (_titleSceneState == TitleSceneState.Title)
        {
            yield return null;

            // ���͂��Ȃ�����
            if (!Input.anyKeyDown) { continue; }

            // �^�C�g����ʂ̏������I��
            MyTitleSceneState = TitleSceneState.TitleClose;            
        }
        */
    }

    /// <summary>
    /// �^�C�g����ʂ���A�V�����V�[����ǂݍ���
    /// </summary>
    private IEnumerator TitleClose()
    {
        Debug.Log("TitleClose");

        // �^�C�g�����S����
        yield return StartCoroutine(TitleLogoSingleton.Instance.TitleLogoBreakAnimation());

        _transition.StartTransition();
    }
}

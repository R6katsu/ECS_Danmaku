using UnityEngine;
using System;
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
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class TitleSceneManager : SingletonMonoBehaviour<TitleSceneManager>
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

    [Tooltip("�^�C�g�����S���j��\�ȏ�Ԃ�")]
    private bool _isTitleLogoDestructible = false;

    [Tooltip("���݂̉摜�ԍ�")]
    private int _currentImageNumber = 0;

    [Tooltip("�j�󂪊���������̏���")]
    public Action breakAction = null;

    [Tooltip("�O��̉�ꂩ���̃^�C�g�����S�摜")]
    private SpriteRenderer _lastTitleLogoBreakImage = null;

    [Tooltip("�^�C�g�����S��Animator")]
    private Animator _titleLogoAnimator = null;

    [SerializeField, Header("��ꂩ���̃^�C�g�����S�摜�̔z��")]
    private SpriteRenderer[] _titleLogoBreakImages = null;

    [SerializeField, Header("�^�C�g�����S�̍Đ�Trigger")]
    private string _titleLogoTriggerName = null;

    [SerializeField, Header("�^�C�g�����S����̍Đ�Trigger")]
    private string _titleLogoBreakTriggerName = null;

    [SerializeField, Min(0), Header("�^�C�g��BGM�̔ԍ�")]
    private int _titleBGMNumber = 0;

    [SerializeField, Header("�V�[���J�ڂ̃g�����W�V�����̖���")]
    private TransitionName _transitionName = 0;

    [Tooltip("�^�C�g���V�[���̏��")]
    private TitleSceneState _titleSceneState = 0;

    /// <summary>
    /// �^�C�g�����S�̓����蔻�肪�L�����ۂ�
    /// </summary>
    public bool HasTitleLogoCollision { get; set; }

    /// <summary>
    /// �^�C�g�����S��Animator
    /// </summary>
    private Animator TitleLogoAnimator
    {
        get
        {
            if (_titleLogoAnimator == null)
            {
                // RequireComponent
                _titleLogoAnimator = GetComponent<Animator>();
            }

            return _titleLogoAnimator;
        }
    }

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
                // �^�C�g�����S�̓����蔻��𖳌���
                HasTitleLogoCollision = false;

                Cursor.lockState = CursorLockMode.Locked;   // �J�[�\������ʒ����Ƀ��b�N����
                Cursor.visible = false;                     // �J�[�\����\��

                // �^�C�g�����S����ꂽ���̏�����o�^
                breakAction = () =>
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
                // �^�C�g�����S�̓����蔻���L����
                HasTitleLogoCollision = true;
                break;

            case TitleSceneState.TitleClose:
                // �^�C�g�����S�̓����蔻��𖳌���
                HasTitleLogoCollision = false;

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
        yield return StartCoroutine(TitleLogoAnimation());

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
        yield return StartCoroutine(TitleLogoBreakAnimation());

        var transition = TransitionDirector.Instance.GetTransition(_transitionName);

        // ���݂̃V�[���̔ԍ����擾
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // ���݂̃V�[���ԍ����C���N�������g
        currentSceneIndex++;

        transition.StartTransition(currentSceneIndex);
    }

    /// <summary>
    /// ���̉�ꂩ���̃^�C�g�����S�摜�ɐ؂�ւ���
    /// </summary>
    /// <returns>���ɐ؂�ւ���摜�����݂��Ȃ��ꍇ��false</returns>
    public bool? NextImage()
    {
        if (!_isTitleLogoDestructible) { return null; }

        // ���ƂŃ��t�@�N�^�����O����
        GetComponent<SpriteRenderer>().enabled = false;

        // ���ɔz��̗v�f���𒴉߂��Ă���
        if (_titleLogoBreakImages.Length <= _currentImageNumber) { return null; }

        for (int i = 0; i < _titleLogoBreakImages.Length; i++)
        {
            if (i != _currentImageNumber) { continue; }

            // ���̉摜�ɐ؂�ւ���
            _titleLogoBreakImages[_currentImageNumber].gameObject.SetActive(true);

            // �O��̉摜�𖳌����A�O��̉摜���X�V
            _lastTitleLogoBreakImage?.gameObject.SetActive(false);
            _lastTitleLogoBreakImage = _titleLogoBreakImages[_currentImageNumber];
        }

        // �摜�̔ԍ����C���N�������g
        _currentImageNumber++;

        // �z��̗v�f���𒴉߂���
        if (_titleLogoBreakImages.Length <= _currentImageNumber) { return false; }

        return true;
    }

    /// <summary>
    /// �^�C�g�����S���Đ�
    /// </summary>
    /// <returns></returns>
    public IEnumerator TitleLogoAnimation()
    {
        // ���ƂŃ��t�@�N�^�����O����
        GetComponent<SpriteRenderer>().enabled = true;

        // �^�C�g�����S�̃A�j���[�V�������Đ�
        yield return UIAnimationAndEndAction.Instance.AnimationAndEndAction
        (
            TitleLogoAnimator,
            _titleLogoTriggerName
        );

        _isTitleLogoDestructible = true;
    }

    /// <summary>
    /// �^�C�g�����S��j�󂵂Ĕj�Ђɂ���
    /// </summary>
    /// <returns></returns>
    public IEnumerator TitleLogoBreakAnimation()
    {
        _isTitleLogoDestructible = false;

        // ���ƂŃ��t�@�N�^�����O����
        GetComponent<SpriteRenderer>().enabled = true;

        // �摜��S�Ė�����
        for (int i = 0; i < _titleLogoBreakImages.Length; i++)
        {
            _titleLogoBreakImages[i].gameObject.SetActive(false);
        }

        // �^�C�g�����S�̔j��A�j���[�V�������Đ�
        yield return UIAnimationAndEndAction.Instance.AnimationAndEndAction
        (
            TitleLogoAnimator,
            _titleLogoBreakTriggerName
        );
    }
}

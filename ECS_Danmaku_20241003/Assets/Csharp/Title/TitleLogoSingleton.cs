using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// �^�C�g�����S�̃V���O���g��
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class TitleLogoSingleton : SingletonMonoBehaviour<TitleLogoSingleton>
{
    [Tooltip("�^�C�g�����S���j��\�ȏ�Ԃ�")]
    private bool _isHFIOAHFIO = false;

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
    /// ���̉�ꂩ���̃^�C�g�����S�摜�ɐ؂�ւ���
    /// </summary>
    /// <returns>���ɐ؂�ւ���摜�����݂��Ȃ��ꍇ��false</returns>
    public bool? NextImage()
    {
        if (!_isHFIOAHFIO) { return null; }

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
    /// <returns>null</returns>
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

        _isHFIOAHFIO = true;
    }

    /// <summary>
    /// �^�C�g�����S��j�󂵂Ĕj�Ђɂ���
    /// </summary>
    /// <returns>null</returns>
    public IEnumerator TitleLogoBreakAnimation()
    {
        _isHFIOAHFIO = false;

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

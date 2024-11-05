using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����Ǘ�
/// </summary>
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField]
    private AudioSource _bgmAudioSource = null;

    [SerializeField]
    private AudioSource _seAudioSource = null;

    [SerializeField, Tooltip("BGM")]
    private AudioClip[] _bgmClips = null;

    [SerializeField, Tooltip("���ʉ�")]
    private AudioClip[] _seClips = null;

    /// <summary>
    /// BGM�Đ�
    /// </summary>
    public void PlayBGM(int num)
    {
        if (_bgmAudioSource == null) { return; }

        _bgmAudioSource.clip = _bgmClips[num];
        _bgmAudioSource.Play();
    }

    /// <summary>
    /// ���ʉ��Đ�
    /// </summary>
    /// <param name="num">�����ԍ�</param>
    public void PlaySE(int num)
    {
        if (_seAudioSource == null) { return; }

        _seAudioSource.PlayOneShot(_seClips[num]);
    }

    /// <summary>
    /// ���ʉ��Đ�
    /// </summary>
    /// <param name="nums">�����ԍ��̔z��</param>
    public void PlaySE(int[] nums)
    {
        if (_seAudioSource == null) { return; }

        StartCoroutine(PlaySEs(nums));
    }

    /// <summary>
    /// �O��̉����Đ����I�������玟�̉������Đ�����
    /// </summary>
    /// <param name="nums">�����ԍ��̔z��</param>
    /// <returns>null</returns>
    private IEnumerator PlaySEs(int[] nums)
    {
        // �O��̉����Đ����I�������玟�̉������Đ�����
        foreach (var num in nums)
        {
            _seAudioSource.clip = _seClips[num];
            _seAudioSource.Play();

            yield return new WaitForSeconds(_seAudioSource.clip.length);
        }
    }
}

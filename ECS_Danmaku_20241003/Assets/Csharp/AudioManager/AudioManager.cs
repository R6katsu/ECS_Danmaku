using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 音源管理
/// </summary>
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField]
    private AudioSource _bgmAudioSource = null;

    [SerializeField]
    private AudioSource _seAudioSource = null;

    [SerializeField, Tooltip("BGM")]
    private AudioClip[] _bgmClips = null;

    [SerializeField, Tooltip("効果音")]
    private AudioClip[] _seClips = null;

    /// <summary>
    /// BGM再生
    /// </summary>
    public void PlayBGM(int num)
    {
        if (_bgmAudioSource == null) { return; }

        _bgmAudioSource.clip = _bgmClips[num];
        _bgmAudioSource.Play();
    }

    /// <summary>
    /// 効果音再生
    /// </summary>
    /// <param name="num">音源番号</param>
    public void PlaySE(int num)
    {
        if (_seAudioSource == null) { return; }

        _seAudioSource.PlayOneShot(_seClips[num]);
    }

    /// <summary>
    /// 効果音再生
    /// </summary>
    /// <param name="nums">音源番号の配列</param>
    public void PlaySE(int[] nums)
    {
        if (_seAudioSource == null) { return; }

        StartCoroutine(PlaySEs(nums));
    }

    /// <summary>
    /// 前回の音源再生が終了したら次の音源を再生する
    /// </summary>
    /// <param name="nums">音源番号の配列</param>
    /// <returns>null</returns>
    private IEnumerator PlaySEs(int[] nums)
    {
        // 前回の音源再生が終了したら次の音源を再生する
        foreach (var num in nums)
        {
            _seAudioSource.clip = _seClips[num];
            _seAudioSource.Play();

            yield return new WaitForSeconds(_seAudioSource.clip.length);
        }
    }
}

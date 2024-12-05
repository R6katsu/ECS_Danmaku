using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Collections;
#endif

// リファクタリング済み

/// <summary>
/// 音源の再生管理
/// </summary>
public class AudioPlayManager : SingletonMonoBehaviour<AudioPlayManager>
{
    [SerializeField]
    private AudioSource _bgmAudioSource = null;

    [SerializeField]
    private AudioSource _seAudioSource = null;

    [SerializeField, Tooltip("BGM")]
    private AudioClip[] _bgmClips = null;

    [SerializeField, Tooltip("SE")]
    private AudioClip[] _seClips = null;

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="num">音源番号</param>
    public void PlayBGM(int num)
    {
        if (_bgmAudioSource == null
            || _bgmClips.Length <= num) { return; }

        // 音源再生
        _bgmAudioSource.clip = _bgmClips[num];
        _bgmAudioSource.Play();
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="num">音源番号</param>
    public void PauseBGM()
    {
        if (_bgmAudioSource == null) { return; }

        // 音源一時停止
        _bgmAudioSource.Pause();
    }

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="num">音源番号</param>
    public void PlaySE(int num)
    {
        if (_seAudioSource == null 
            || _seClips.Length <= num) { return; }

        // PlayOneShotで音源再生
        _seAudioSource.PlayOneShot(_seClips[num]);
    }
}

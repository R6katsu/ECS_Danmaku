using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using static VFXCreationBridge;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// 音源再生に必要な情報
/// </summary>
public struct AudioPlayData : IComponentData
{
    private const int LOWER_LIMIT = -1;

    private int _audioNumber;
    private bool _isPlaying;

    [Tooltip("音源の種類")]
    public readonly AudioType audioType;

    /// <summary>
    /// 再生する音源の番号。<br/>
    /// 下限から変更があった場合に「音源再生」のフラグを立てる
    /// </summary>
    public int AudioNumber
    {
        get
        {
            // 無効化前の音源番号を保持
            var audioNumber = _audioNumber;

            // 音源再生を無効にする
            DisableAudioPlay();

            return audioNumber;
        }
        set
        {
            // 下限から変更があった場合に「生成可能」のフラグを立てる
            if (_audioNumber != value && value != LOWER_LIMIT)
            {
                _audioNumber = value;
                _isPlaying = true;
            }
        }
    }

    /// <summary>
    /// 音源再生可能かどうかのフラグ
    /// </summary>
    public bool IsPlaying => _isPlaying;

    /// <summary>
    /// 音源再生の情報
    /// </summary>
    /// <param name="audioType">音源の種類</param>
    public AudioPlayData(AudioType audioType)
    {
        this.audioType = audioType;

        _audioNumber = LOWER_LIMIT;
        _isPlaying = false;
    }

    /// <summary>
    /// 音源再生を無効にする
    /// </summary>
    public void DisableAudioPlay()
    {
        _audioNumber = LOWER_LIMIT;
        _isPlaying = false;
    }
}

/// <summary>
/// 音源再生に必要な設定
/// </summary>
public class AudioPlayAuthoring : MonoBehaviour
{
    [SerializeField, Header("音源の種類")]
    private AudioType _audioType = 0;

    public class Baker : Baker<AudioPlayAuthoring>
    {
        public override void Bake(AudioPlayAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // 自身のEntityにAudioPlayDataをアタッチ
            AddComponent(entity, new AudioPlayData(src._audioType));
        }
    }
}

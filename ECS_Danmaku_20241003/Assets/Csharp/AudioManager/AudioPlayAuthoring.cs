using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using static VFXCreationBridge;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// 音源の種類
/// </summary>
public enum AudioType
{
    BGM,
    SE
}

/// <summary>
/// 音源再生の情報
/// </summary>
public struct AudioPlayData : IComponentData
{
    private const int LOWER_LIMIT = -1;

    private int audioNumber;
    private bool isPlaying;

    [Tooltip("音源の種類")]
    public readonly AudioType audioType;

    /// <summary>
    /// 再生する音源の番号。<br/>
    /// 下限から変更があった場合に「音源再生」のフラグを立てる
    /// </summary>
    public int AudioNumber
    {
        get => audioNumber;
        set
        {
            // 下限から変更があった場合に「生成可能」のフラグを立てる
            if (audioNumber != value && value != LOWER_LIMIT)
            {
                audioNumber = value;
                isPlaying = true;
            }
        }
    }

    /// <summary>
    /// 音源再生可能かどうかのフラグ
    /// </summary>
    public bool IsPlaying => isPlaying;

    /// <summary>
    /// 音源再生の情報
    /// </summary>
    public AudioPlayData(AudioType audioType)
    {
        this.audioType = audioType;

        audioNumber = LOWER_LIMIT;
        isPlaying = false;
    }

    /// <summary>
    /// 音源再生を無効にする
    /// </summary>
    public void DisableAudioPlay()
    {
        audioNumber = LOWER_LIMIT;
        isPlaying = false;
    }
}

/// <summary>
/// 音源再生の設定
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

            AddComponent(entity, new AudioPlayData(src._audioType));
        }
    }
}

using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using Unity.Transforms;
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// 音源の再生処理。<br/>
/// 音源の再生をメインスレッドで行うためのSystem
/// </summary>
public partial struct AudioPlaySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // 音源の再生処理
        foreach (var audioPlay in
                 SystemAPI.Query<RefRW<AudioPlayData>>())
        {
            // 再生可能フラグが立っていなければコンテニュー
            if (!audioPlay.ValueRO.IsPlaying) { continue; }

            // 音源再生処理を呼び出す
            CallAudioPlay(audioPlay.ValueRO.audioType, audioPlay);
        }
    }

    /// <summary>
    /// 音源再生処理を呼び出す
    /// </summary>
    /// <param name="audioType">音源の種類</param>
    /// <param name="audioPlay">音源再生に必要な情報</param>
    private void CallAudioPlay(AudioType audioType, RefRW<AudioPlayData> audioPlay)
    {
        switch (audioType)
        {
            case AudioType.BGM:
                AudioPlayManager.Instance.PlayBGM(audioPlay.ValueRW.AudioNumber);
                break;

            case AudioType.SE:
                AudioPlayManager.Instance.PlaySE(audioPlay.ValueRW.AudioNumber);
                break;

            case AudioType.None:
            default:
#if UNITY_EDITOR
                Debug.LogError("audioTypeが未設定、または例外");
#endif
                break;
        }
    }
}

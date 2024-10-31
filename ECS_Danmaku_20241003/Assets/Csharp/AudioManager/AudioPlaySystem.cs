using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// 音源再生
/// </summary>
public partial struct AudioPlaySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // メインスレッドで関数を呼び出す為のSystem
        foreach (var audioPlay in
                 SystemAPI.Query<RefRW<AudioPlayData>>())
        {
            // 再生可能フラグが立っていなければコンテニュー
            if (!audioPlay.ValueRO.IsPlaying) { continue; }

            if (audioPlay.ValueRO.audioType == AudioType.BGM)
            {
                AudioManager.Instance.PlayBGM(audioPlay.ValueRO.AudioNumber);
            }
            else
            {
                AudioManager.Instance.PlaySE(audioPlay.ValueRO.AudioNumber);
            }

            // 再生を無効にする
            audioPlay.ValueRW.DisableAudioPlay();
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

#if UNITY_EDITOR
using UnityEngine.UIElements;
using UnityEditor;
using System.Drawing;
using System.Collections;
using Unity.Mathematics;
using Unity.Entities;
using static VFXCreationBridge;
#endif

// リファクタリング済み

/// <summary>
/// VisualEffectの種類
/// </summary>
public enum VisualEffectType : sbyte
{
    None = -1,
    Explosion,
    Charge,
}

/// <summary>
/// VFXを生成する。VFXとECSを連携させる架け橋となるclass
/// </summary>
public class VFXCreationBridge : SingletonMonoBehaviour<VFXCreationBridge>
{
    [Tooltip("デフォルトの大きさ")]
    private const float DEFAULT_SIZE = 1.0f;

    [SerializeField, Header("VFXGraphの配列")]
    private VisualEffect[] _visualEffects = null;

    [Tooltip("位置のリストの辞書")]
    private Dictionary<VisualEffectType, List<Vector3>> _positions = new();

    [Tooltip("大きさのリストの辞書")]
    private Dictionary<VisualEffectType, List<float>> _sizes = new();

    [Tooltip("位置のGraphicsBufferの辞書")]
    private Dictionary<VisualEffectType, GraphicsBuffer> _positionBuffers = new();

    [Tooltip("大きさのGraphicsBufferの辞書")]
    private Dictionary<VisualEffectType, GraphicsBuffer> _sizeBuffers = new();

    private void OnDestroy()
    {
        // GraphicsBufferの解放
        foreach (var buffer in _positionBuffers.Values)
        {
            buffer.Dispose();
        }

        foreach (var buffer in _sizeBuffers.Values)
        {
            buffer.Dispose();
        }
    }

    /// <summary>
    /// VFXGraphを生成する
    /// </summary>
    /// <param name="visualEffectName">生成するVFXの種類</param>
    /// <param name="position">生成する位置</param>
    /// <param name="size">生成時の大きさ</param>
    public void VFXCreation(VisualEffectType visualEffectName, Vector3 position, float size = DEFAULT_SIZE)
    {
        if (visualEffectName == VisualEffectType.None)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Noneが渡されました");
#endif
            return;
        }

        // 既に追加されていたら追加しない
        if (!_positions.ContainsKey(visualEffectName))
        {
            _positions.Add(visualEffectName, new List<Vector3>()); 
        }

        if (!_sizes.ContainsKey(visualEffectName))
        { 
            _sizes.Add(visualEffectName, new List<float>());
        }

        // 書き込む為のリストに追加
        _positions[visualEffectName].Add(position);
        _sizes[visualEffectName].Add(size);

        // ここから書き込み処理
        _positionBuffers[visualEffectName] = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _positions[visualEffectName].Count, sizeof(float) * 3);
        _sizeBuffers[visualEffectName] = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _sizes[visualEffectName].Count, sizeof(float));

        _positionBuffers[visualEffectName].SetData(_positions[visualEffectName]);
        _sizeBuffers[visualEffectName].SetData(_sizes[visualEffectName]);

        var visualEffect = _visualEffects[(sbyte)visualEffectName];
        visualEffect.SetGraphicsBuffer("PositionBuffer", _positionBuffers[visualEffectName]);
        visualEffect.SetGraphicsBuffer("SizeBuffer", _positionBuffers[visualEffectName]);

        // 生成を開始
        visualEffect.SendEvent("OnPlay");
    }
}

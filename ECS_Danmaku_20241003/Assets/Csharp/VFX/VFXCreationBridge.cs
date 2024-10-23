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

/// <summary>
/// VFXを生成する。VFXとECSを連携させる架け橋となるclass
/// </summary>
public class VFXCreationBridge : SingletonMonoBehaviour<VFXCreationBridge>
{
    [SerializeField, Header("VFXGraphの配列")]
    private VisualEffect[] _visualEffects = null;

    [Tooltip("位置のリストの辞書")]
    private Dictionary<VisualEffectName, List<Vector3>> _positions = new();

    [Tooltip("大きさのリストの辞書")]
    private Dictionary<VisualEffectName, List<float>> _sizes = new();

    [Tooltip("位置のGraphicsBufferの辞書")]
    private Dictionary<VisualEffectName, GraphicsBuffer> _positionBuffers = new();

    [Tooltip("大きさのGraphicsBufferの辞書")]
    private Dictionary<VisualEffectName, GraphicsBuffer> _sizeBuffers = new();

    /// <summary>
    /// VisualEffectの名称。<br/>
    /// 配列と連携する場合、0番目はNoneであるため要素数は-1すること。
    /// </summary>
    public enum VisualEffectName : byte
    {
        None = 0,
        Test20241017Graph,
    }

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
    /// <param name="visualEffectName">生成するVFXの名称</param>
    /// <param name="position">生成する位置</param>
    /// <param name="size">生成時の大きさ（初期値は1）</param>
    public void VFXCreation(VisualEffectName visualEffectName, Vector3 position, float size)
    {
        if (visualEffectName == VisualEffectName.None)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Noneが渡されました");
#endif
            return;
        }

        if (!_positions.ContainsKey(visualEffectName))
        { _positions.Add(visualEffectName, new List<Vector3>()); }

        if (!_sizes.ContainsKey(visualEffectName))
        { _sizes.Add(visualEffectName, new List<float>()); }

        _positions[visualEffectName].Add(position);
        _sizes[visualEffectName].Add(size);

        _positionBuffers[visualEffectName] = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _positions[visualEffectName].Count, sizeof(float) * 3);
        _sizeBuffers[visualEffectName] = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _sizes[visualEffectName].Count, sizeof(float));

        _positionBuffers[visualEffectName].SetData(_positions[visualEffectName]);
        _sizeBuffers[visualEffectName].SetData(_sizes[visualEffectName]);

        var visualEffect = _visualEffects[(byte)visualEffectName - 1];
        visualEffect.SetGraphicsBuffer("PositionBuffer", _positionBuffers[visualEffectName]);
        visualEffect.SetGraphicsBuffer("SizeBuffer", _positionBuffers[visualEffectName]);

        visualEffect.SendEvent("OnPlay");
    }
}

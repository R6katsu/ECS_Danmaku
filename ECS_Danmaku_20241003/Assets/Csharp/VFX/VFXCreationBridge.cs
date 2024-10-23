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
/// VFX�𐶐�����BVFX��ECS��A�g������˂����ƂȂ�class
/// </summary>
public class VFXCreationBridge : SingletonMonoBehaviour<VFXCreationBridge>
{
    [SerializeField, Header("VFXGraph�̔z��")]
    private VisualEffect[] _visualEffects = null;

    [Tooltip("�ʒu�̃��X�g�̎���")]
    private Dictionary<VisualEffectName, List<Vector3>> _positions = new();

    [Tooltip("�傫���̃��X�g�̎���")]
    private Dictionary<VisualEffectName, List<float>> _sizes = new();

    [Tooltip("�ʒu��GraphicsBuffer�̎���")]
    private Dictionary<VisualEffectName, GraphicsBuffer> _positionBuffers = new();

    [Tooltip("�傫����GraphicsBuffer�̎���")]
    private Dictionary<VisualEffectName, GraphicsBuffer> _sizeBuffers = new();

    /// <summary>
    /// VisualEffect�̖��́B<br/>
    /// �z��ƘA�g����ꍇ�A0�Ԗڂ�None�ł��邽�ߗv�f����-1���邱�ƁB
    /// </summary>
    public enum VisualEffectName : byte
    {
        None = 0,
        Test20241017Graph,
    }

    private void OnDestroy()
    {
        // GraphicsBuffer�̉��
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
    /// VFXGraph�𐶐�����
    /// </summary>
    /// <param name="visualEffectName">��������VFX�̖���</param>
    /// <param name="position">��������ʒu</param>
    /// <param name="size">�������̑傫���i�����l��1�j</param>
    public void VFXCreation(VisualEffectName visualEffectName, Vector3 position, float size)
    {
        if (visualEffectName == VisualEffectName.None)
        {
#if UNITY_EDITOR
            Debug.LogWarning("None���n����܂���");
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

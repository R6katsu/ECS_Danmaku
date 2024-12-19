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

// ���t�@�N�^�����O�ς�

/// <summary>
/// VisualEffect�̎��
/// </summary>
public enum VisualEffectType : sbyte
{
    None = -1,
    Explosion,
    Charge,
}

/// <summary>
/// VFX�𐶐�����BVFX��ECS��A�g������˂����ƂȂ�class
/// </summary>
public class VFXCreationBridge : SingletonMonoBehaviour<VFXCreationBridge>
{
    [Tooltip("�f�t�H���g�̑傫��")]
    private const float DEFAULT_SIZE = 1.0f;

    [SerializeField, Header("VFXGraph�̔z��")]
    private VisualEffect[] _visualEffects = null;

    [Tooltip("�ʒu�̃��X�g�̎���")]
    private Dictionary<VisualEffectType, List<Vector3>> _positions = new();

    [Tooltip("�傫���̃��X�g�̎���")]
    private Dictionary<VisualEffectType, List<float>> _sizes = new();

    [Tooltip("�ʒu��GraphicsBuffer�̎���")]
    private Dictionary<VisualEffectType, GraphicsBuffer> _positionBuffers = new();

    [Tooltip("�傫����GraphicsBuffer�̎���")]
    private Dictionary<VisualEffectType, GraphicsBuffer> _sizeBuffers = new();

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
    /// <param name="visualEffectName">��������VFX�̎��</param>
    /// <param name="position">��������ʒu</param>
    /// <param name="size">�������̑傫��</param>
    public void VFXCreation(VisualEffectType visualEffectName, Vector3 position, float size = DEFAULT_SIZE)
    {
        if (visualEffectName == VisualEffectType.None)
        {
#if UNITY_EDITOR
            Debug.LogWarning("None���n����܂���");
#endif
            return;
        }

        // ���ɒǉ�����Ă�����ǉ����Ȃ�
        if (!_positions.ContainsKey(visualEffectName))
        {
            _positions.Add(visualEffectName, new List<Vector3>()); 
        }

        if (!_sizes.ContainsKey(visualEffectName))
        { 
            _sizes.Add(visualEffectName, new List<float>());
        }

        // �������ވׂ̃��X�g�ɒǉ�
        _positions[visualEffectName].Add(position);
        _sizes[visualEffectName].Add(size);

        // �������珑�����ݏ���
        _positionBuffers[visualEffectName] = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _positions[visualEffectName].Count, sizeof(float) * 3);
        _sizeBuffers[visualEffectName] = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _sizes[visualEffectName].Count, sizeof(float));

        _positionBuffers[visualEffectName].SetData(_positions[visualEffectName]);
        _sizeBuffers[visualEffectName].SetData(_sizes[visualEffectName]);

        var visualEffect = _visualEffects[(sbyte)visualEffectName];
        visualEffect.SetGraphicsBuffer("PositionBuffer", _positionBuffers[visualEffectName]);
        visualEffect.SetGraphicsBuffer("SizeBuffer", _positionBuffers[visualEffectName]);

        // �������J�n
        visualEffect.SendEvent("OnPlay");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Unity.VisualScripting;

#if UNITY_EDITOR
#endif

/// <summary>
/// UI�I��
/// </summary>
public class UIController : MonoBehaviour
{
    /// <summary>
    /// ���������̓��͉񐔂�UnityEvent�̏��
    /// </summary>
    [Serializable]
    public struct AxisInputEventInfo
    {
        [SerializeField, Header("���������̓��͉�")]
        private int2 _axisInput;

        [SerializeField, Header("���������̓��͉񐔂ƕR���ČĂяo�����֐�")]
        private UnityEvent _axisInputEvent;

        public int2 AxisInput => _axisInput;
        public UnityEvent AxisInputEvent => _axisInputEvent;
    }

    [SerializeField, Header("���������̓��͉񐔂�UnityEvent�̏��̔z��")]
    private AxisInputEventInfo[] _axisInputEventInfos = null;

    private UnityEvent[,] _unityEvents = null;

    [Tooltip("���͕����ɕ��̒l���܂܂�Ă��邩")]
    private bool _hasNegativeValue = false;

    /// <summary>
    /// ���������̓��͉񐔂�UnityEvent�̏��̑������z��
    /// </summary>
    private UnityEvent[,] AxisInputEventInfos
    {
        get
        {
            // �������z��null������
            if (_unityEvents == null)
            {
                // �������z����쐬���A�\���̓���int2�𑽎����z��ɒu��������
                var xMax = _axisInputEventInfos.Max(info => info.AxisInput.x);
                var yMax = _axisInputEventInfos.Max(info => info.AxisInput.y);

                _unityEvents = new UnityEvent[xMax, yMax];

                foreach (var info in _axisInputEventInfos)
                {
                    _unityEvents[info.AxisInput.x, info.AxisInput.y] = info.AxisInputEvent;
                }
            }

            return _unityEvents;
        }
    }

    public UnityEvent GetANHFAJON(int2 key)
    {
        UnityEvent unityEvent = null;

        // �ŏ��l���擾
        var xMin = _axisInputEventInfos.Min(info => info.AxisInput.x);
        var yMin = _axisInputEventInfos.Min(info => info.AxisInput.y);

        // �e�����̂����ꂩ�̍ŏ��l�����̒l�������ꍇ�A
        // "���̒l���܂܂�Ă��邩"�Ƃ����t���O�𗧂Ă�
        _hasNegativeValue = (xMin < 0 || yMin < 0) ? true : false;

        //  �擾�\�ȃC�x���g�̓��͕����ɕ��̒l���܂܂�Ă���
        if (_hasNegativeValue)
        {
            // ����r���Ĉ�v������̂�T������
            foreach (var axisInputEventInfo in _axisInputEventInfos)
            {
                unityEvent = 
                    (axisInputEventInfo.AxisInput.x == key.x &&
                    axisInputEventInfo.AxisInput.y == key.y) ?
                        axisInputEventInfo.AxisInputEvent : null;
            }

            // ��v�������̂�����΂����Ԃ��A�������null��Ԃ�
            return unityEvent;
        }
        else
        {
            // �������z����쐬���ĕԂ��A���������v�f�������l�̏ꍇ��null��Ԃ�
            return AxisInputEventInfos[key.x, key.y];
        }
    }
}

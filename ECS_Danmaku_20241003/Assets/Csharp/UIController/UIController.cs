using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Unity.VisualScripting;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
#endif

/// <summary>
/// UI�I��
/// </summary>
public class UIController : MonoBehaviour, IDisposable
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

    [Tooltip("�I�𒆂�UI�C�x���g")]
    private int2 _selectedUIEvent = int2.zero;

    [Tooltip("UI�̑��삪�L����")]
    private bool _isUIEnabled = false;

    // InputSystem
    private PlayerControls _playerInput;

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

    /// <summary>
    /// ����̗L����
    /// </summary>
    public void Enable()
    {
        // �{�^���ƑI�𒆂̃{�^���̋���
        // �͈͊O�ɏo�����ɔ��Α��ɖ߂��Ă��鏈��

        // �ŏ��l���擾
        var xMin = _axisInputEventInfos.Min(info => info.AxisInput.x);
        var yMin = _axisInputEventInfos.Min(info => info.AxisInput.y);

        // �e�����̂����ꂩ�̍ŏ��l�����̒l�������ꍇ�A
        // "���̒l���܂܂�Ă��邩"�Ƃ����t���O�𗧂Ă�
        _hasNegativeValue = (xMin < 0 || yMin < 0) ? true : false;

        // ���C���X���b�h�ŏ�������Action�ɓo�^
        MainThreadExecutor.Instance.Enqueue
        (() =>
        {
            // ������
            _selectedUIEvent = int2.zero;

            _playerInput = new PlayerControls();
            _playerInput.Enable();

            // Shot�Ɋ��蓖�Ă�
            var confirm = _playerInput.UI.Confirm;
            confirm.started += (context) =>
            {
                var uiEvent = GetANHFAJON();

                // ���C���X���b�h�ŏ�������Action�ɓo�^
                MainThreadExecutor.Instance.Enqueue
                (() =>
                {
                    uiEvent?.Invoke();
                }
                );
            };

            // Horizontal�Ɋ��蓖�Ă�
            var horizontal = _playerInput.UI.Horizontal;
            horizontal.started += (context) =>
            {
                float inputValue = context.ReadValue<float>();

                if (inputValue > 0)
                { // �E�����̓���
                    SelectedHorizontalIncrement();
                }
                else if (inputValue < 0)
                { // �������̓���
                    SelectedHorizontalDecrement();
                }
            };

            // Vertical�Ɋ��蓖�Ă�
            var vertical = _playerInput.UI.Vertical;
            vertical.started += (context) =>
            {
                float inputValue = context.ReadValue<float>();

                if (inputValue > 0)
                { // ������̓���
                    SelectedVerticalIncrement();
                }
                else if (inputValue < 0)
                { // �������̓���
                    SelectedVerticalDecrement();
                }
            };

            // ����̗L����
            _isUIEnabled = true;
        }
        );
    }

    // IDisposable
    public void Dispose()
    {
        if (_playerInput != null)
        {
            // ����̖������A���\�[�X���
            _playerInput.Disable();
            _playerInput.Dispose();
        }

        // ����̖�����
        _isUIEnabled = false;
    }

    public void OnDisable()
    {
        Dispose();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public UnityEvent GetANHFAJON()
    {
        // UI�̑��삪����������
        if (!_isUIEnabled) { return null; }

        UnityEvent unityEvent = null;

        //  �擾�\�ȃC�x���g�̓��͕����ɕ��̒l���܂܂�Ă���
        if (_hasNegativeValue)
        {
            // ����r���Ĉ�v������̂�T������
            foreach (var axisInputEventInfo in _axisInputEventInfos)
            {
                if (axisInputEventInfo.AxisInput.x == _selectedUIEvent.x &&
                    axisInputEventInfo.AxisInput.y == _selectedUIEvent.y)
                {
                    unityEvent = axisInputEventInfo.AxisInputEvent;
                }
            }

            // ��v�������̂�����΂����Ԃ��A�������null��Ԃ�
            return unityEvent;
        }
        else
        {
            // �������z����쐬���ĕԂ��A���������v�f�������l�̏ꍇ��null��Ԃ�
            return AxisInputEventInfos[_selectedUIEvent.x, _selectedUIEvent.y];
        }
    }

    /// <summary>
    /// �������͂��C���N�������g
    /// </summary>
    public void SelectedHorizontalIncrement() => _selectedUIEvent.x++;

    /// <summary>
    /// �������͂��f�N�������g
    /// </summary>
    public void SelectedHorizontalDecrement() => _selectedUIEvent.x--;

    /// <summary>
    /// �������͂��C���N�������g
    /// </summary>
    public void SelectedVerticalIncrement() => _selectedUIEvent.y++;

    /// <summary>
    /// �������͂��f�N�������g
    /// </summary>
    public void SelectedVerticalDecrement() => _selectedUIEvent.y--;
}

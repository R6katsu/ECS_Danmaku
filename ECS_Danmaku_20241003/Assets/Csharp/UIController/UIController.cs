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

        [SerializeField, Header("�{�^���摜")]
        private RectTransform _buttonImage;

        [SerializeField, Header("���������̓��͉񐔂ƕR���ČĂяo�����֐�")]
        private UnityEvent _axisInputEvent;

        public int2 AxisInput => _axisInput;
        public RectTransform ButtonImage => _buttonImage;
        public UnityEvent AxisInputEvent => _axisInputEvent;
        public (RectTransform, UnityEvent) ButtonImage_AxisInputEvent => (_buttonImage, _axisInputEvent);
    }

    [SerializeField, Header("���������̓��͉񐔂�UnityEvent�̏��̔z��")]
    private AxisInputEventInfo[] _axisInputEventInfos = null;

    private (RectTransform, UnityEvent)[,] _unityEvents = null;

    [Tooltip("���͕����ɕ��̒l���܂܂�Ă��邩")]
    private bool _hasNegativeValue = false;

    [Tooltip("�I�𒆂�UI�C�x���g")]
    private int2 _selectedUIEvent = int2.zero;

    [Tooltip("UI�̑��삪�L����")]
    private bool _isUIEnabled = false;

    // InputSystem
    private PlayerControls _playerInput;

    // �e�����̍ő�l�A�ŏ��l
    private int _xValueMax = 0;
    private int _yValueMax = 0;
    private int _xValueMin = 0;
    private int _yValueMin = 0;

    /// <summary>
    /// ���������̓��͉񐔂�UnityEvent�̏��̑������z��
    /// </summary>
    private (RectTransform, UnityEvent)[,] AxisInputEventInfos
    {
        get
        {
            // �������z��null������
            if (_unityEvents == null)
            {
                // �������z����쐬���A�\���̓���int2�𑽎����z��ɒu��������
                _unityEvents = new (RectTransform, UnityEvent)[_xValueMax, _yValueMax];

                foreach (var info in _axisInputEventInfos)
                {
                    _unityEvents[info.AxisInput.x, info.AxisInput.y] = info.ButtonImage_AxisInputEvent;
                }
            }

            return _unityEvents;
        }
    }

    private void OnEnable()
    {
        // ���ɗL��������
        if (_isUIEnabled) { return; }

        // �{�^���摜�𖳌���
        foreach (var info in _axisInputEventInfos)
        {
            info.ButtonImage_AxisInputEvent.Item1.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ����̗L����
    /// </summary>
    public void Enable()
    {
        // �ő�l���擾
        _xValueMax = _axisInputEventInfos.Max(info => info.AxisInput.x);
        _yValueMax = _axisInputEventInfos.Max(info => info.AxisInput.y);

        // �ŏ��l���擾
        _xValueMin = _axisInputEventInfos.Min(info => info.AxisInput.x);
        _yValueMin = _axisInputEventInfos.Min(info => info.AxisInput.y);

        // �e�����̂����ꂩ�̍ŏ��l�����̒l�������ꍇ�A
        // "���̒l���܂܂�Ă��邩"�Ƃ����t���O�𗧂Ă�
        _hasNegativeValue = (_xValueMin < 0 || _yValueMin < 0) ? true : false;

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
                UnityEvent uiEvent = GetANHFAJON().Value.Item2;

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

            // �{�^���摜��L����
            foreach (var info in _axisInputEventInfos)
            {
                info.ButtonImage_AxisInputEvent.Item1.gameObject.SetActive(true);
            }

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
    public (RectTransform, UnityEvent)? GetANHFAJON()
    {
        // UI�̑��삪����������
        if (!_isUIEnabled) { return null; }

        (RectTransform, UnityEvent) unityEvent = new();

        //  �擾�\�ȃC�x���g�̓��͕����ɕ��̒l���܂܂�Ă���
        if (_hasNegativeValue)
        {
            // ����r���Ĉ�v������̂�T������
            foreach (var axisInputEventInfo in _axisInputEventInfos)
            {
                if (axisInputEventInfo.AxisInput.x == _selectedUIEvent.x &&
                    axisInputEventInfo.AxisInput.y == _selectedUIEvent.y)
                {
                    unityEvent = axisInputEventInfo.ButtonImage_AxisInputEvent;
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
    public void SelectedHorizontalIncrement()
    {
        AAAAA();

        _selectedUIEvent.x++;

        // x�����̍ő�l�𒴉߂����ꍇ�͍ŏ��l����
        _selectedUIEvent.x = (_selectedUIEvent.x > _xValueMax) ? _xValueMin : _selectedUIEvent.x;

        BBBBB();
    }

    /// <summary>
    /// �������͂��f�N�������g
    /// </summary>
    public void SelectedHorizontalDecrement()
    {
        AAAAA();

        _selectedUIEvent.x--;

        // x�����̍ŏ��l�����������ꍇ�͍ő�l����
        _selectedUIEvent.x = (_selectedUIEvent.x < _xValueMin) ? _xValueMax : _selectedUIEvent.x;

        BBBBB();
    }

    /// <summary>
    /// �������͂��C���N�������g
    /// </summary>
    public void SelectedVerticalIncrement()
    {
        AAAAA();

        _selectedUIEvent.y++;

        // y�����̍ő�l�𒴉߂����ꍇ�͍ŏ��l����
        _selectedUIEvent.y = (_selectedUIEvent.y > _yValueMax) ? _yValueMin : _selectedUIEvent.y;

        BBBBB();
    }

    /// <summary>
    /// �������͂��f�N�������g
    /// </summary>
    public void SelectedVerticalDecrement()
    {
        AAAAA();

        _selectedUIEvent.y--;

        // y�����̍ŏ��l�����������ꍇ�͍ő�l����
        _selectedUIEvent.y = (_selectedUIEvent.y < _yValueMin) ? _yValueMax : _selectedUIEvent.y;

        BBBBB();
    }

    private void AAAAA()
    {
        RectTransform buttonImage = GetANHFAJON().Value.Item1;
        buttonImage.localScale = Vector2.one;
    }

    private void BBBBB()
    {
        // �O��I�����Ă����摜�����߂đ傫�������ɖ߂�
        // ����I�𒆂̃{�^����傫������
        RectTransform buttonImage = GetANHFAJON().Value.Item1;
        buttonImage.localScale = Vector2.one * 1.5f;
    }
}

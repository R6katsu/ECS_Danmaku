using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoxCollider = UnityEngine.BoxCollider;

#if UNITY_EDITOR
using Unity.Physics;
#endif

/// <summary>
/// �w�i�X�N���[��
/// </summary>
public class BackGroundScroll : MonoBehaviour
{
    /// <summary>
    /// ���̎��
    /// </summary>
    public enum AxisType
    { X, Y, Z }

    [SerializeField, Header("�X�N���[�����������")]
    private AxisType _axisType = 0;

    [SerializeField, Header("�X�N���[��������w�i")]
    private Transform _backGround = null;

    [SerializeField, Header("�ڕW�܂ł̒���")]
    private float _targetLength = 0.0f;

    [SerializeField, Header("�X�N���[�����x")]
    private float _scrollSpeed = 0.0f;

    [Tooltip("�X�N���[������������̎��̑傫��")]
    private float _axisScale = 0.0f;

    [Tooltip("�ړ�����")]
    private Vector3 _moveDirection = Vector3.zero;

    [Tooltip("�����ʒu")]
    private Vector3 _initialPosition = Vector3.zero;

    private float _loopTargetLength = 0.0f;

    private void OnEnable()
    {
        // �X�N���[��������w�i��null������
        if (_backGround == null)
        {
            enabled = false;
#if UNITY_EDITOR
            Debug.LogError("�X�N���[��������w�i��null������");
#endif
            return;
        }

        // �X�N���[�����������΂�����
        if (_scrollSpeed < 0.0f)
        {
            // �e�̖��O���`
            var parentName = "ParentBackScroll";

            // �e���쐬
            var parentBackScroll = new GameObject();
            parentBackScroll.name = parentName;

            // ���΂̒�`
            var faceOppositeDirection = Vector3.up * 180.0f;

            // �e��ݒ�
            _backGround.transform.parent = parentBackScroll.transform;

            // ���΂���������
            parentBackScroll.transform.rotation = Quaternion.Euler(faceOppositeDirection);

            // ���̒l�ɕύX
            _scrollSpeed *= -1.0f;
        }

        _initialPosition = _backGround.transform.position;

        _loopTargetLength = -_targetLength;

        switch (_axisType)
        {
            case AxisType.X:
                _axisScale = _backGround.localScale.x;
                _moveDirection = Vector3.right * _scrollSpeed;
                break;

            case AxisType.Y:
                _axisScale = _backGround.localScale.y;
                _moveDirection = Vector3.up * _scrollSpeed;
                break;

            case AxisType.Z:
                _axisScale = _backGround.localScale.z;
                _moveDirection = Vector3.forward * _scrollSpeed;
                break;
        }
    }

    private void Update()
    {
        var aaa = Time.deltaTime * _moveDirection;

        bool aajioa = false;
        Vector3 gafaaa = Vector3.zero;

        switch (_axisType)
        {
            case AxisType.X:
                aajioa = _initialPosition.x + _targetLength <= _backGround.transform.localPosition.x + aaa.x;
                gafaaa = _initialPosition + _loopTargetLength * Vector3.right;
                break;

            case AxisType.Y:
                aajioa = _initialPosition.y + _targetLength <= _backGround.transform.localPosition.y + aaa.y;
                gafaaa = _initialPosition + _loopTargetLength * Vector3.up;
                break;

            case AxisType.Z:
                aajioa = _initialPosition.z + _targetLength <= _backGround.transform.localPosition.z + aaa.z;
                gafaaa = _initialPosition + _loopTargetLength * Vector3.forward;
                break;
        }

        if (aajioa)
        {
            _backGround.transform.localPosition = gafaaa;
        }
        else
        {
            _backGround.transform.localPosition += aaa;
        }
    }
}

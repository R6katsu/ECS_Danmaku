using UnityEngine;


#if UNITY_EDITOR
using Unity.Physics;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
#endif

/// <summary>
/// �w�i�X�N���[��
/// </summary>
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class BackGroundScroll : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // ���F
        Gizmos.color = Color.yellow;

        // ���������w�i�̏o���ʒu
        Gizmos.DrawWireCube(transform.position + _backGroundSize.z * Vector3.forward, _backGroundSize);
        Gizmos.DrawWireCube(transform.position + _backGroundSize.z * Vector3.back, _backGroundSize);

        // ��
        Gizmos.color = Color.green;

        // �I���W�i���̔w�i�̑傫��
        Gizmos.DrawWireCube(transform.position, _backGroundSize);
    }

    [SerializeField, Header("�X�N���[�����������")]
    private AxisType _axisType = 0;

    [SerializeField, Header("�X�N���[��������w�i")]
    private Transform _backGround = null;

    [SerializeField, Header("�X�N���[�����x")]
    private float _scrollSpeed = 0.0f;

    [SerializeField, Header("�w�i�̑傫��")]
    private Vector3 _backGroundSize = Vector3.zero;

    [Tooltip("�ړ�����")]
    private Vector3 _moveDirection = Vector3.zero;

    [Tooltip("�����ʒu")]
    private Vector3 _initialPosition = Vector3.zero;

    [Tooltip("�ړ��J�n�ʒu")]
    private Vector3 _startPosition = Vector3.zero;

    [Tooltip("�����ʒu����ړ��I���ʒu�܂ł̋���")]
    private float _endLength = 0.0f;

    private void OnEnable()
    {
        // �w�i��null������
        if (_backGround == null)
        {
#if UNITY_EDITOR
            // �V�����w�i�𐶐�����
            _backGround = InstantiateBackGround("BackGround", Vector3.zero);
#else
            enabled = false;
            return;
#endif
        }

#if UNITY_EDITOR
        // �����^�C�����ł͂Ȃ�����
        if (!Application.isPlaying) { return; }
#endif

        // �w�i��O��ɕ���
        InstantiateBackGround("ForwardBackGround", transform.position + _backGroundSize.z * Vector3.forward, _backGround);
        InstantiateBackGround("BackBackGround", transform.position + _backGroundSize.z * Vector3.back, _backGround);

        // �����ʒu
        _initialPosition = transform.position;

        // �����ʒu����ړ��J�n/�I���ʒu�܂ł̋��������߂�
        _endLength = _backGroundSize.z;

        // AxisType�ɑΉ�����������擾
        var axisValue = AxisTypeHelper.GetAxisDirection(_axisType);

        // �����Ƒ��x
        _moveDirection = axisValue * _scrollSpeed;

        // �J�n�ʒu
        Vector3 startPosition = -_endLength * axisValue;

        // �ڕW�ƊJ�n�ʒu�̕����𔽑΂ɂ���
        _startPosition = (_scrollSpeed < 0.0f) ? _initialPosition - startPosition : _initialPosition + startPosition;
    }

    private void Update()
    {
#if UNITY_EDITOR
        // �����^�C�����ł͂Ȃ�����
        if (!Application.isPlaying) { return; }
#endif
        var endLength = _endLength;

        // ����̃t���[���ł̈ړ���
        var frameMovement = Time.deltaTime * _moveDirection;

        // �ڕW�ɓ��B������
        bool hasReachedTarget = false;

        // �ړ����������]���Ă��邩
        bool isMovingBackwards = _scrollSpeed < 0.0f;

        // ���]���Ă����畄���𔽓]������
        endLength *= (isMovingBackwards) ? -1.0f : 1.0f;

        // AxisType�ɑΉ�����e�������擾
        var initialPositionValue = AxisTypeHelper.GetAxisValue(_axisType, _initialPosition);
        var localPositionValue = AxisTypeHelper.GetAxisValue(_axisType, transform.position);
        var frameMovementValue = AxisTypeHelper.GetAxisValue(_axisType, frameMovement);

        // �ړ������ɂ���ďI�������ƌ��݂̈ړ�������ς���
        float end = (isMovingBackwards) ? initialPositionValue + endLength : localPositionValue + frameMovementValue;
        float current = (isMovingBackwards) ? localPositionValue + frameMovementValue : initialPositionValue + endLength;

        // �ڕW�ɓ��B������
        hasReachedTarget = end >= current;

        // �ڕW�ɓ��B����
        if (hasReachedTarget)
        {
            // �ړ��J�n�ʒu�Ɉړ�
            transform.position = _startPosition;
        }
        else
        {
            // �ړ��𔽉f
            transform.position += frameMovement;
        }
    }

    /// <summary>
    /// �w�i�𐶐�����
    /// </summary>
    /// <param name="name">�����A�܂��͕��������Ώۂ̖��O</param>
    /// <param name="position">�����ʒu</param>
    /// <param name="original">��������ꍇ�̃I���W�i��</param>
    /// <returns>�����A�܂��͕��������Ώ�</returns>
    private Transform InstantiateBackGround(string name, Vector3 position, Transform original = null)
    {
        // �����Ώۂ�null��������V���������A�Ⴆ�Ε���
        var backGround = (original == null) ? new GameObject().transform : Instantiate(original);

        // �����ݒ�
        backGround.name = name;
        backGround.transform.parent = transform;
        backGround.localPosition = position;

        return backGround;
    }
}

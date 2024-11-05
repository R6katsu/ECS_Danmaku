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

    /// <summary>
    /// ���̎��
    /// </summary>
    private enum AxisType
    { X, Y, Z }

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
#if UNITY_EDITOR
        // �����^�C�����ł͂Ȃ�����
        if (!Application.isPlaying) { return; }
#endif

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

        // �w�i��O��ɕ���
        InstantiateBackGround("ForwardBackGround", transform.position + _backGroundSize.z * Vector3.forward, _backGround);
        InstantiateBackGround("BackBackGround", transform.position + _backGroundSize.z * Vector3.back, _backGround);

        // �X�N���[�����������΂�����
        if (_scrollSpeed < 0.0f)
        {
            // �e�̖��O���`
            var parentName = "ParentBackScroll";

            // �e���쐬
            var parentBackScroll = new GameObject();
            parentBackScroll.name = parentName;

            // ���Ό����̒�`
            var faceOppositeDirection = Vector3.up * 180.0f;

            // �e��ݒ�
            transform.parent = parentBackScroll.transform;

            // ���΂���������
            parentBackScroll.transform.rotation = Quaternion.Euler(faceOppositeDirection);

            // ���̒l�ɕύX
            _scrollSpeed *= -1.0f;
        }

        // �����ʒu
        _initialPosition = transform.position;

        // �����ʒu����ړ��J�n/�I���ʒu�܂ł̋��������߂�
        _endLength = _backGroundSize.z;

        // �ړ�����
        switch (_axisType)
        {
            case AxisType.X:
                _moveDirection = Vector3.right * _scrollSpeed;
                _startPosition = _initialPosition + -_endLength * Vector3.right;
                break;

            case AxisType.Y:
                _moveDirection = Vector3.up * _scrollSpeed;
                _startPosition = _initialPosition + -_endLength * Vector3.up;
                break;

            case AxisType.Z:
                _moveDirection = Vector3.forward * _scrollSpeed;
                _startPosition = _initialPosition + -_endLength * Vector3.forward;
                break;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        // �����^�C�����ł͂Ȃ�����
        if (!Application.isPlaying) { return; }
#endif

        // ����̃t���[���ł̈ړ���
        var frameMovement = Time.deltaTime * _moveDirection;

        // �ڕW�ɓ��B������
        bool hasReachedTarget = false;

        switch (_axisType)
        {
            case AxisType.X:
                hasReachedTarget = _initialPosition.x + _endLength <= transform.localPosition.x + frameMovement.x;
                break;

            case AxisType.Y:
                hasReachedTarget = _initialPosition.y + _endLength <= transform.localPosition.y + frameMovement.y;
                break;

            case AxisType.Z:
                hasReachedTarget = _initialPosition.z + _endLength <= transform.localPosition.z + frameMovement.z;
                break;
        }

        // �ڕW�ɓ��B����
        if (hasReachedTarget)
        {
            // �ړ��J�n�ʒu�Ɉړ�
            transform.localPosition = _startPosition;
        }
        else
        {
            // �ړ��𔽉f
            transform.localPosition += frameMovement;
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

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using Child = Unity.Transforms.Child;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
using Unity.VisualScripting;



#if UNITY_EDITOR
using static BulletHelper;
#endif

/// <summary>
/// �w�i�X�N���[���̏��
/// </summary>
public struct BackGroundScrollData : IComponentData
{
    [Tooltip("�X�N���[�����������")]
    public readonly AxisType axisType;

    [Tooltip("�ړ�����")]
    public readonly float3 moveDirection;

    [Tooltip("�����ʒu")]
    public readonly float3 initialPosition;

    [Tooltip("�ړ��J�n�ʒu")]
    public readonly float3 startPosition;

    [Tooltip("�����ʒu����ړ��I���ʒu�܂ł̋���")]
    public readonly float endLength;

    /// <summary>
    /// �w�i�X�N���[���̏��
    /// </summary>
    /// <param name="axisType">�X�N���[�����������</param>
    /// <param name="moveDirection">�ړ�����</param>
    /// <param name="initialPosition">�����ʒu</param>
    /// <param name="startPosition">�ړ��J�n�ʒu</param>
    /// <param name="endLength">�����ʒu����ړ��I���ʒu�܂ł̋���</param>
    public BackGroundScrollData
        (
            AxisType axisType,
            float3 moveDirection,
            float3 initialPosition,
            float3 startPosition,
            float endLength
        )
    {
        this.axisType = axisType;
        this.moveDirection = moveDirection;
        this.initialPosition = initialPosition;
        this.startPosition = startPosition;
        this.endLength = endLength;
    }
}

/// <summary>
/// �w�i�X�N���[���̐ݒ�
/// </summary>
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class BackGroundScrollAuthoring : MonoBehaviour
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

    [SerializeField, Header("�X�N���[�����x")]
    private float _scrollSpeed = 0.0f;

    [SerializeField, Header("�w�i�̑傫��")]
    private Vector3 _backGroundSize = Vector3.zero;

    [SerializeField, Header("�X�N���[��������w�i")]
    private Transform _backGround = null;

    [SerializeField, Header("�O���ɕ��������w�i")]
    private Transform _forwardBackGround = null;

    [SerializeField, Header("����ɕ��������w�i")]
    private Transform _backBackGround = null;

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
        var scrollSpeed = _scrollSpeed;

        // �w�i��null������
        if (_backGround == null)
        {
#if UNITY_EDITOR
            // �V�����w�i�𐶐�
            _backGround = InstantiateBackGround("BackGround", Vector3.zero);
#else
            enabled = false;
            return;
#endif
        }

#if UNITY_EDITOR
        if (_forwardBackGround == null)
        {
            // �w�i��O���ɕ���
            _forwardBackGround = InstantiateBackGround("ForwardBackGround", transform.position + _backGroundSize.z * Vector3.forward, _backGround);
        }
        if (_backBackGround == null)
        {
            // �w�i������ɕ���
            _backBackGround = InstantiateBackGround("BackBackGround", transform.position + _backGroundSize.z * Vector3.back, _backGround);
        }
#else
            enabled = false;
            return;
#endif
        // �����ʒu
        _initialPosition = transform.position;

        // �����ʒu����ړ��J�n/�I���ʒu�܂ł̋��������߂�
        _endLength = _backGroundSize.z;

        // AxisType�ɑΉ�����������擾
        var axisValue = AxisTypeHelper.GetAxisDirection(_axisType);

        // �����Ƒ��x
        _moveDirection = axisValue * scrollSpeed;

         // �J�n�ʒu
        Vector3 startPosition = -_endLength * axisValue;

        // �ڕW�ƊJ�n�ʒu�̕����𔽑΂ɂ���
        _startPosition = (scrollSpeed < 0.0f) ? _initialPosition - startPosition : _initialPosition + startPosition;
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

    public class Baker : Baker<BackGroundScrollAuthoring>
    {
        public override void Bake(BackGroundScrollAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var backGroundScrollData = new BackGroundScrollData
                (
                    src._axisType,
                    src._moveDirection,
                    src._initialPosition,
                    src._startPosition,
                    src._endLength
                );

            AddComponent(entity, backGroundScrollData);
        }
    }
}

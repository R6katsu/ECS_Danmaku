using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// �ΏۂɌ������Ē����ړ����鏈��
/// </summary>
[BurstCompile]
public partial struct HomingMoveSystm : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        // �Ώۂ͂ǂ�����Ďw�肷��H
        // �����ł͑ΏۂɌ������Ē����ړ����邾��
        // �Ώۂ͑��̏ꏊ�Ŏw�肷��
        // ����A���Ɏw�肪�Ȃ��ꍇ�͎��g�ƈقȂ�w�c�̒������ԋ߂��Ώۂ�_���΂�����

        // campsType

        // �ΏۂɌ������Ē����ړ����鏈��
        foreach (var (homing, localTfm) in
                 SystemAPI.Query<RefRO<HomingMoveData>,
                                 RefRW<LocalTransform>>())
        {
            // �Ώۂ�null������
            if (homing.ValueRO.targetEntity == Entity.Null)
            {
                // �ł��߂��قȂ�w�c��Entity��ΏۂƂ���

                // ���ʂɍl���āA�S�Ă�Entity����w�c���擾���ċ������v�Z������āA
                // �S�Ă̒ǔ��e������������d���Ȃ���
                // �w�c����List���쐬����H�@�e�ƓG�őΏۂ��Ⴄ���A������l������
                // ���̏ꏊ�Őw�c����Entity���Ǘ����A�����T�����ċ��������߂�H
                // �e���A�G���őΏۂ��قȂ�BMove�����łȂ��A�G���e����enum�������Ŕ��ʂ�����
                // �e�A��Q���A�G�i�����j��Tag��������Ƃ�������
                // ����͐���+�قȂ�w�c�����ԂɒT������K�v������
                // �w�c��enum�ł͂Ȃ�Data�ɂ���iTag�j�Benum�͂����܂ł��C���X�y�N�^����ݒ肷�鎞�����g�p����
            }

            // ���[�J�����W�n���g���A�����Ă�������ւ̈ړ����v�Z
            //float3 forward = math.forward(localTfm.ValueRO.Rotation);

            // �ړ��𔽉f
            //localTfm.ValueRW.Position += forward * facingDirection.ValueRO.moveParam.Speed * delta;
        }
    }
}

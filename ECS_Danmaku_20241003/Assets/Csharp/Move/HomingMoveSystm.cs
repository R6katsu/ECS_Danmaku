using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;
using static EntityCampsHelper;
using static EntityCategoryHelper;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// �ΏۂɌ������Ē����ړ����鏈��
/// </summary>
[BurstCompile]
public partial struct HomingMoveSystm : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        Debug.LogError("�����ɗ\��������Ă���");

        // �G�ɒe�������������A�e�ɂ���Ă͏�����悤�ɂ���
        // �ђʂȂǂ�enum�Ō��߂邩�B����Ƃ��񐔁H


        // �f�B���N�g���ɂ��Ă�#if�������Čy�ʉ�����
        // Authoring��get�v���p�e�B�𖳂�����private�����ɂ���
        // �w�c��Authoring�Őݒ肷��̂ł͂Ȃ��A�����Ƃ��ē���Ă��炤�H
        // ����A�����̐w�c���A���m�E���������ꍇ�͎��g�̏�������Entity����w�cData���擾���đ������悤�ɐ݌v����
        // �w�c�����ł͂Ȃ��AEntity�̎�ށi�����A����A�e���A��Q��etc...�j�ɂ��Ă�Tag����������

        // Tag�ɂ��ẮA���g�̏�������Entity����Data���擾������@���l����



        // �Ώۂ͂ǂ�����Ďw�肷��H
        // �����ł͑ΏۂɌ������Ē����ړ����邾��
        // �Ώۂ͑��̏ꏊ�Ŏw�肷��
        // ����A���Ɏw�肪�Ȃ��ꍇ�͎��g�ƈقȂ�w�c�̒������ԋ߂��Ώۂ�_���΂�����

        // campsType

        // �ΏۂɌ������Ē����ړ����鏈��
        foreach (var (homing, velocity, localTfm) in
                 SystemAPI.Query<RefRO<HomingMoveData>,
                                 RefRW<PhysicsVelocity>,
                                 RefRW<LocalTransform>>())
        {
            //continue;   // �������̈�

            // �Ώۂ�null������
            if (homing.ValueRO.targetEntity == Entity.Null)
            {
                // �擾����Entity���g���������͑S�ē���
                // �������v�Z���Ĉ�ԋ߂������ڎw���΂����B����͋���
                // �����ƌ����΁AEntity�ł͂Ȃ��A�Ή�����Tag���������z�̈ʒu��񂪗~��������

                // ����A����Entity�����S�Ă�Data���擾���Ă����r����̂��Ȃ񂾂��Ȃ����Ċ���
                // ����ς��x�Őw�cTag�ƃJ�e�S��Tag����v��������
                // �܂������B�Ƃ肠�����G�Ɍ������e�������������悤

                float maxjpjfaiopfjaiop = float.MaxValue;
                float3 gjniknhiaogkvihaoTfm = new float3();    // ��ԋ߂�
                float3 currentPoint = localTfm.ValueRO.Position;

                if (homing.ValueRO.targetCampsType == EntityCampsType.Enemy && homing.ValueRO.targetEntityCategory == EntityCategory.LivingCreature)
                {
                    foreach (var (livingCreatureCategory, enemyCamps, targetLocalTfm) in
                    SystemAPI.Query<RefRO<LivingCreatureCategoryTag>,
                                    RefRO<EnemyCampsTag>,
                                    RefRW<LocalTransform>>())
                    {
                        // �ڕW������ & �G�w�c������

                        var targetPoint = targetLocalTfm.ValueRO.Position;
                        var dis = math.distance(targetPoint, currentPoint);

                        // �ł��߂��Ƃ���鋗�����X�V����
                        if (dis < maxjpjfaiopfjaiop)
                        {
                            maxjpjfaiopfjaiop = dis;
                            gjniknhiaogkvihaoTfm = targetPoint;
                        }
                    }
                }
                else
                {
                    Debug.Log("������");
                }

                // ���݈ʒu����ڕW�ʒu�ւ̃x�N�g�����v�Z
                float3 direction = math.normalize(gjniknhiaogkvihaoTfm - currentPoint);

                /*
                // �ړ����鋗�����v�Z
                float distanceToMove = homing.ValueRO.moveParam.Speed * delta;

                // �ړ���K�p
                localTfm.ValueRW.Position += direction * distanceToMove;
                */

                // �Ȃ�Ƃ������A�]�C�݂����Ȃ̂�����������

                // �^�[�Q�b�g�̈ʒu���擾
                float3 targetPosition = gjniknhiaogkvihaoTfm;

                // ���݂̒e�̈ʒu�ƃ^�[�Q�b�g�̈ʒu�Ƃ̍����v�Z
                float3 directionToTarget = direction;

                // ���݂̒e�̑��x�����Ƀ^�[�Q�b�g�������������āA�V���������֑��x�𒲐�
                float3 newVelocity = math.lerp(velocity.ValueRO.Linear, directionToTarget * homing.ValueRO.moveParam.Speed, homing.ValueRO.moveParam.Speed * delta);

                // �V�������x��ݒ�
                velocity.ValueRW.Linear = newVelocity;



                // �ړ��̓��͂��擾�i��F�E�Ɉړ�����ꍇ�j
                //velocity.ValueRW.Linear.x = moveSpeed; // x�����Ɉړ�
                //velocity.ValueRW.Linear.y = 0f;
                //velocity.ValueRW.Linear.z = 0f;
            }
        }
    }
}

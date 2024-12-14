using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �e�������E�\
/// </summary>
public struct BulletEraseTag : IComponentData { }

/// <summary>
/// �e�������E�\�̐ݒ�
/// </summary>
public class BulletEraseAuthoring : MonoBehaviour
{
    public class Baker : Baker<BulletEraseAuthoring>
    {
        public override void Bake(BulletEraseAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BulletEraseTag());
        }
    }
}

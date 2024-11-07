using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using static HealthPointDatas;
using static PlayerHelper;
#endif

/// <summary>
/// �r���{�[�h
/// </summary>
public struct BillboardTag : IComponentData { }

/// <summary>
/// �r���{�[�h�̐ݒ�
/// </summary>
public class BillboardAuthoring : MonoBehaviour
{
    public class Baker : Baker<BillboardAuthoring>
    {
        public override void Bake(BillboardAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new BillboardTag());
        }
    }
}

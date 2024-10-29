using Unity.Entities;
using UnityEngine;
using static HealthPointDatas;

#if UNITY_EDITOR
using static PlayerHelper;
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// PL‚ÌHP‚Ìİ’è
/// </summary>
public class PlayerHealthPointAuthoring : MonoBehaviour
{
    [SerializeField, Min(0.0f), Header("Å‘å‘Ì—Í")]
    private float _maxHP = 0.0f;

    [SerializeField, Min(0.0f), Header("–³“GŠÔ‚Ì’·‚³")]
    private float _isInvincibleTime = 0.0f;

    public class Baker : Baker<PlayerHealthPointAuthoring>
    {
        public override void Bake(PlayerHealthPointAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PlayerHealthPointData(src._maxHP, src._isInvincibleTime));

            src.enabled = false;
        }
    }
}

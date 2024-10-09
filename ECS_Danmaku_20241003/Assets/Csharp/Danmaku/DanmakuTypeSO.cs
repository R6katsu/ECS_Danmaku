using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DanmakuTypeSO", menuName = "ScriptableObject/DanmakuTypeSO")]
public class DanmakuTypeSO : ScriptableObject
{
    [SerializeField, Header("’ePrefab")]
    private Transform _bulletPrefab = null;

    public Transform BulletPrefab => _bulletPrefab;
}

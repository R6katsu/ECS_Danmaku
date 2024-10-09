using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Entities;
using System.Reflection;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using static MoveHelper;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    // SpawneLineCreateにBakeを入れている為、こちらが機能しない
    // このScriptをアタッチしたらSpawneLineCreateをアタッチするのもなんか違う
    // そもそも二つに分けない方法を考えた方がいいかもしれない


    void OnDrawGizmos()
    {
        if (_startPoint != null && _endPoint != null)
        {
            // 線の色を設定
            Gizmos.color = Color.red;

            // 始点と終点を結ぶ線を描画
            Gizmos.DrawLine(_startPoint.position, _endPoint.position);
        }
    }

    [SerializeField, Header("生成した敵の移動方向")]
    private DirectionTravelType _directionTravelType = 0;

    [SerializeField, Header("生成する位置の線を書く為の始点と終点を設定する")]
    protected Transform _startPoint = null;
    [SerializeField]
    protected Transform _endPoint = null;

    [SerializeField, Header("生成する敵のPrefab配列")]
    private GameObject[] _enemyPrefabs = null;

    public DirectionTravelType MyDirectionTravelType => _directionTravelType;
    public Transform StartPoint => _startPoint;
    public Transform EndPoint => _endPoint;
    public GameObject[] EnemyPrefabs => _enemyPrefabs;
}
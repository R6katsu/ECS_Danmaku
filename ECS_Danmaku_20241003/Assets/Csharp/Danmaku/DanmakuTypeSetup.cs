#if UNITY_EDITOR
#if false
using System.ComponentModel;
#endif
using UnityEditor;
using UnityEngine;
using static DanmakuHelper;
#endif

/// <summary>
/// DanmakuTypeに変更があった時、必要なComponentのSetupを行う。
/// ビルド後は空のMonoBehaviourになる。
/// </summary>
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public sealed class DanmakuTypeSetup : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private DanmakuType _danmakuType = 0;

    // 前回の弾幕の種類を保持
    private DanmakuType _previousDanmakuType = 0;

    // Prefabから設置した際の処理を通過したか
    private bool _isPrefabPlaced = false;

    // インスペクタから値が変更された
    private void OnValidate()
    {
        // Prefabから設置した
        if (!_isPrefabPlaced)
        {
            _isPrefabPlaced = true;
            _previousDanmakuType = _danmakuType;
            return;
        }

        SetupDanmakuAuthoring(_danmakuType);
    }

    private void OnEnable()
    {
        // ランタイム中は非有効化
        if (Application.isPlaying)
            enabled = false;
    }

    /// <summary>
    /// DanmakuTypeの内容に応じて必要なComponentのSetupを行う
    /// </summary>
    /// <param name="danmakuType">弾幕の種類</param>
    private void SetupDanmakuAuthoring(DanmakuType danmakuType)
    {
        // 前回から変わっていなければ返す
        if (_previousDanmakuType == _danmakuType) { return; }

        // 変わっていれば更新
        _previousDanmakuType = _danmakuType;

        // ランタイム中は無効
        if (Application.isPlaying) { return; }
        // 
        // Prefabだった場合は変更せず返す
        if (EditorUtility.IsPersistent(this))
        {
            // Prefabを選択中に変更があった場合はメッセージ出力
            if (Selection.activeGameObject == this.gameObject)
                Debug.LogError("シーンに配置してから設定してください");

            return;
        }

        // 自身にアタッチされているComponentを探索
        // IDanmakuAuthoringを継承していたら破壊
        foreach (var component in GetComponents<Component>())
        {
            // IDanmakuAuthoringを継承していない場合はコンテニュー
            if (!(component is IDanmakuAuthoring danmakuAuthoring)) { continue; }

            // ランタイム中なら破壊、elseなら遅延後に即時破壊
            if (Application.isPlaying)
                Destroy(component);
            else
                EditorApplication.delayCall += () =>
                {
                    if (this != null)
                        DestroyImmediate(component);
                };
        }

        // DanmakuTypeの内容に応じて必要なComponentをアタッチ
        switch (danmakuType)
        {
            default:
            case DanmakuType.None:
                break;

            case DanmakuType.N_Way:
                // ランタイム中なら追加、elseなら遅延後に追加
                if (Application.isPlaying)
                    gameObject.AddComponent<N_Way_DanmakuAuthoring>();
                else
                    EditorApplication.delayCall += () =>
                    {
                        if (this != null)
                            gameObject.AddComponent<N_Way_DanmakuAuthoring>();
                    };
                break;
        }
    }
#endif
}
using UnityEditor;
using UnityEngine;
using static DanmakuHelper;
using Component = UnityEngine.Component;

#if UNITY_EDITOR
using System.ComponentModel;
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
    [SerializeField, Header("弾幕の種類")]
    private DanmakuType _danmakuType = 0;

    [Tooltip("前回の弾幕の種類を保持")]
    private DanmakuType _previousDanmakuType = 0;

    [Tooltip("Prefabから設置した際の処理を通過したかのフラグ")]
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

        // DanmakuTypeの内容に応じて必要なComponentのSetupを行う
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
        // 前回から変わっていなければ切り上げる
        if (_previousDanmakuType == _danmakuType) { return; }

        // ランタイム中は無効
        if (Application.isPlaying) { return; }
        // 
        // Prefabだった場合は変更せず返す
        if (EditorUtility.IsPersistent(this))
        {
            // Prefabを選択中に変更があった場合はメッセージ出力
            if (Selection.activeGameObject == this.gameObject)
            { Debug.LogError("シーンに配置してから設定してください"); }

            return;
        }

        // 変わっていれば更新を反映
        _previousDanmakuType = _danmakuType;

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
            // 例外なら何もしない
            default:
            case DanmakuType.None:
                break;

            case DanmakuType.N_Way:
                AddComponent<N_Way_DanmakuAuthoring>();
                break;
            case DanmakuType.TapShooting:
                AddComponent<TapShooting_DanmakuAuthoring>();
                break;
        }
    }

    /// <summary>
    /// ランタイム中か否かの判定を含んだAddComponent
    /// </summary>
    /// <typeparam name="T">AddComponentする型</typeparam>
    private void AddComponent<T>() where T : MonoBehaviour
    {
        // ランタイム中なら追加、elseなら遅延後に追加
        if (Application.isPlaying)
            gameObject.AddComponent<T>();
        else
            EditorApplication.delayCall += () =>
            {
                if (this != null)
                    gameObject.AddComponent<T>();
            };
    }
#endif
}
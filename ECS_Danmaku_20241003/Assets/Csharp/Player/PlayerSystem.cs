using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using static BulletHelper;
using Unity.Physics;
using static HealthPointDatas;
using System.Collections;

#if UNITY_EDITOR
using UnityEngine.Rendering;
using static EntityCampsHelper;
using Unity.Burst;
using static UnityEngine.Rendering.DebugUI;
using Unity.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine.SocialPlatforms.Impl;
using static HealthHelper;
#endif

// リファクタリング済み

/// <summary>
/// 接触時にダメージを受ける
/// </summary>
public partial struct PlayerDamageTriggerJob : ITriggerEventsJob
{
    // JobでDataを取得、変更する為のComponentLookup
    public ComponentLookup<PlayerSingletonData> playerSingletonLookup;
    public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
    public ComponentLookup<DestroyableData> destroyableLookup;
    public ComponentLookup<LocalTransform> localTransformLookup;
    public ComponentLookup<VFXCreationData> vfxCreationLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        var entityA = triggerEvent.EntityA; // 接触対象
        var entityB = triggerEvent.EntityB; // isTriggerを有効にしている方

        // entityBがLocalTransformを有しているか
        if (!localTransformLookup.HasComponent(entityB)) { return; }

        // isTriggerが有効である以上、LocalTransformは絶対にある
        var localTransform = localTransformLookup[entityB];
        var position = localTransform.Position;

        // entityAがBulletIDealDamageDataを有していない。
        // あるいは、entityBがPlayerSingletonDataを有していなければ切り上げる
        if (!dealDamageLookup.HasComponent(entityA) || !playerSingletonLookup.HasComponent(entityB)) { return; }

        // entityBからPlayerHealthPointDataを取得
        var playerSingleton = playerSingletonLookup[entityB];

        // entityAからBulletIDealDamageDataを取得
        var dealDamage = dealDamageLookup[entityA];

        // ダメージ源の陣営の種類がPlayerだったら切り上げる
        if (dealDamage.campsType == EntityCampsType.Player) { return; }

        // 攻撃を受けた対象がDestroyableDataを有していた
        if (destroyableLookup.HasComponent(entityB))
        {
            var destroyable = destroyableLookup[entityB];

            // 削除フラグを代入する
            destroyable.isKilled = true;

            // 変更を反映
            destroyableLookup[entityB] = destroyable;

            // 削除フラグが立った
            // これはEntityが削除される時の処理として追加できないか
            if (destroyable.isKilled)
            {
                // ゲームオーバー処理を開始する
                GameManager.Instance.MyGameState = GameState.GameOver;

                // EntityBがVFXCreationDataを有していた
                if (vfxCreationLookup.HasComponent(entityB))
                {
                    var vfxCreation = vfxCreationLookup[entityB];
                    vfxCreation.Position = position;
                    vfxCreationLookup[entityB] = vfxCreation;
                }
            }
        }
    }
}

/// <summary>
/// PLの処理
/// </summary>
public partial class PlayerSystem : SystemBase
{
    // JobでDataを取得、変更する為のComponentLookup
    private ComponentLookup<PlayerSingletonData> _playerSingletonLookup = new();
    private ComponentLookup<HealthPointData> _healthPointLookup = new();
    private ComponentLookup<BulletIDealDamageData> _dealDamageLookup = new();
    private ComponentLookup<DestroyableData> _destroyableLookup = new();
    private ComponentLookup<RemainingPierceCountData> _remainingPierceCountLookup = new();
    private ComponentLookup<LocalTransform> _localTransformLookup = new();
    private ComponentLookup<VFXCreationData> _vfxCreationLookup = new();

    [Tooltip("PL入力のInputSystem")]
    private PlayerControls _playerInput = null;

    [Tooltip("水平入力")]
    private float _horizontalValue = 0.0f;

    [Tooltip("垂直入力")]
    private float _verticalValue = 0.0f;

    [Tooltip("溜め攻撃中")]
    private bool _isChargeShot = false;

    [Tooltip("減速中")]
    private bool _isSlowdown = false;

    [Tooltip("最後に攻撃が当たった時間")]
    private double _lastShotTime = 0.0f;

    [Tooltip("チャージを開始した時間")]
    private double _chargeStartTime = 0.0f;

    /// <summary>
    /// Jobの為のGetComponentLookupを作成。<br/>
    /// PlayerInputのインスタンス作成と入力時の関数を登録。
    /// </summary>
    protected override void OnCreate()
    {
        // 取得する
        _playerSingletonLookup = GetComponentLookup<PlayerSingletonData>(false);
        _healthPointLookup = GetComponentLookup<HealthPointData>(false);
        _dealDamageLookup = GetComponentLookup<BulletIDealDamageData>(false);
        _destroyableLookup = GetComponentLookup<DestroyableData>(false);
        _remainingPierceCountLookup = GetComponentLookup<RemainingPierceCountData>(false);
        _localTransformLookup = GetComponentLookup<LocalTransform>(false);
        _vfxCreationLookup = GetComponentLookup<VFXCreationData>(false);

        // PlayerControlsをインスタンス化し、有効にする
        // リソース解放の為にフィールド変数として保持する
        _playerInput = new PlayerControls();
        _playerInput.Enable();

        // ChargeShotに割り当てる
        var chargeShot = _playerInput.Player.ChargeShot;
        chargeShot.started += ChargeShotStarted;
        chargeShot.canceled += ChargeShotCanceled;

        // Horizontalに割り当てる
        var horizontal = _playerInput.Player.Horizontal;
        horizontal.started += Horizontal;
        horizontal.started += TiltOnMoveStart;
        horizontal.canceled += Horizontal;
        horizontal.canceled += TiltOnMoveEnd;

        // Verticalに割り当てる
        var vertical = _playerInput.Player.Vertical;
        vertical.started += Vertical;
        vertical.canceled += Vertical;

        // Slowdownに割り当てる
        var slowdown = _playerInput.Player.Slowdown;
        slowdown.started += (context) => _isSlowdown = true;
        slowdown.canceled += (context) => _isSlowdown = false;
    }

    /// <summary>
    /// ISystemを継承していると呼び出されるUpdate
    /// </summary>
    protected override void OnUpdate()
    {
        // PlayerSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // シングルトンデータの取得
        var playerSingleton = SystemAPI.GetSingleton<PlayerSingletonData>();

        // PlayerSingletonDataを持つEntityを取得
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerSingletonData>();

        // LocalTransformを所持していなかった
        if (!SystemAPI.HasComponent<LocalTransform>(playerEntity)) { return; }

        // PLからLocalTransformを取得
        var playerTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        var delta = SystemAPI.Time.DeltaTime;       // フレーム秒数
        var elapsed = SystemAPI.Time.ElapsedTime;   // 経過時間

        // 移動処理
        PlayerMove(delta, playerEntity, playerSingleton, playerTransform);

        if (!_isChargeShot)
        {
            // 射撃処理
            PlayerShot(elapsed, playerEntity, playerSingleton, playerTransform);
        }

        // PLの被弾時の処理
        PlayerDamageTriggerJob();
    }

    /// <summary>
    /// インスタンスが破棄される時に呼び出される
    /// </summary>
    protected override void OnDestroy()
    {
        _playerInput.Disable();     // 非有効化
        _playerInput.Dispose();     // リソース解放
    }

    /// <summary>
    /// チャージを開始
    /// </summary>
    /// <param name="context"></param>
    private void ChargeShotStarted(InputAction.CallbackContext context)
    {
        // チャージショットのフラグを有効化
        _isChargeShot = true;

        // チャージを開始した時間を保持する
        _chargeStartTime = SystemAPI.Time.ElapsedTime;

        // PlayerSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // シングルトンデータの取得
        var playerSingletonData = SystemAPI.GetSingleton<PlayerSingletonData>();

        // チャージエフェクトを生成する処理を予約
        // VFXCreationBridgeが存在しなければ生成できないのでMonoもInstanceから取得
        VFXCreationBridge.Instance.StartCoroutine(VFXCreation(playerSingletonData.chargeTime));
    }

    /// <summary>
    /// チャージエフェクトを生成する処理を呼び出す
    /// </summary>
    private IEnumerator VFXCreation(float wait)
    {
        // フレーム秒
        var delta = 0.0f;

        // 時間が経過するまで待機
        while (wait >= delta)
        {
            yield return null;

            // フレーム秒を加算代入
            delta += SystemAPI.Time.DeltaTime;

            // 途中でチャージが終了した
            if (!_isChargeShot) { yield break; }
        }

        // PlayerSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { yield break; }

        // シングルトンデータの取得
        var playerSingletonData = SystemAPI.GetSingleton<PlayerSingletonData>();

        // PlayerSingletonDataを持つEntityを取得
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerSingletonData>();

        // PLがLocalTransformを所持していなかった
        if (!SystemAPI.HasComponent<LocalTransform>(playerEntity)) { yield break; }

        // PLのLocalTransformを取得
        var playerTfm = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        // チャージが完了した時のエフェクトを生成する
        VFXCreationBridge.Instance.VFXCreation(VisualEffectType.Charge, playerTfm.Position);

        // チャージが完了した時の効果音を再生する
        AudioPlayManager.Instance.PlaySE(playerSingletonData.chargeFinishedSE);
    }

    /// <summary>
    /// チャージを終了
    /// </summary>
    /// <param name="context"></param>
    private void ChargeShotCanceled(InputAction.CallbackContext context)
    {
        // チャージショットのフラグを無効化
        _isChargeShot = false;

        // チャージ時間の長さ
        var chargeTime = SystemAPI.Time.ElapsedTime - _chargeStartTime;

        // PlayerSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // シングルトンデータの取得
        var playerSingletonData = SystemAPI.GetSingleton<PlayerSingletonData>();

        // PlayerSingletonDataを持つEntityを取得
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerSingletonData>();

        // チャージ時間が足りなかった
        if (chargeTime < playerSingletonData.chargeTime)
        {
            // チャージショットが不発だった時の効果音を再生する
            AudioPlayManager.Instance.PlaySE(playerSingletonData.chargeShotMissSE);
            return;
        }

        // 弾を生成
        var bulletEntity = EntityManager.Instantiate(playerSingletonData.chargeBulletEntity);

        // PLがLocalTransformを所持していなかった
        if (!SystemAPI.HasComponent<LocalTransform>(playerEntity)) { return; }

        // PLのLocalTransformを取得
        var playerTfm = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        // 弾がLocalTransformを所持していなかった
        if (!SystemAPI.HasComponent<LocalTransform>(bulletEntity)) { return; }

        // 弾のLocalTransformを取得
        var bulletTfm = SystemAPI.GetComponent<LocalTransform>(bulletEntity);

        // 位置をPLの位置に設定
        bulletTfm = new LocalTransform()
        {
            Position = playerTfm.Position,
            Rotation = playerTfm.Rotation,
            Scale = bulletTfm.Scale
        };

        // 弾の変更を反映
        EntityManager.SetComponentData(bulletEntity, bulletTfm);

        // チャージショットの効果音を再生する
        AudioPlayManager.Instance.PlaySE(playerSingletonData.chargeShotSE);
    }

    /// <summary>
    /// 水平入力の処理
    /// </summary>
    /// <param name="context"></param>
    private void Horizontal(InputAction.CallbackContext context)
    {
        _horizontalValue = context.ReadValue<float>();
    }

    /// <summary>
    /// 垂直入力の処理
    /// </summary>
    /// <param name="context"></param>
    private void Vertical(InputAction.CallbackContext context)
    {
        _verticalValue = context.ReadValue<float>();
    }

    /// <summary>
    /// PLの移動処理
    /// </summary>
    /// <param name="delta">現在のフレーム秒数</param>
    /// <param name="playerEntity">PLのEntity</param>
    /// <param name="playerSingleton">PLのSingletonData</param>
    /// <param name="playerTransform">PLのLocalTransform</param>
    public void PlayerMove(float delta, Entity playerEntity, PlayerSingletonData playerSingleton, LocalTransform playerTransform)
    {
        // 入力がなかった
        if (_verticalValue == 0.0f && _horizontalValue == 0.0f) { return; }

        // MovementRangeSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<MovementRangeSingletonData>()) { return; }

        // シングルトンデータの取得
        var movementRangeSingleton = SystemAPI.GetSingleton<MovementRangeSingletonData>();

        // PLの移動可能範囲を取得
        var playerMovementRange = movementRangeSingleton.PlayerMovementRange;

        // 中心位置
        var movementRangeCenter = playerMovementRange.movementRangeCenter;

        // 半分の大きさを求める
        var halfMovementRange = playerMovementRange.movementRange.Halve();

        // 中心位置を考慮した移動可能範囲を求める
        var minMovementRange = movementRangeCenter + -halfMovementRange;
        var maxMovementRange = movementRangeCenter + halfMovementRange;

        // 現在地を取得
        var currentPosition = playerTransform.Position;

        // 移動速度
        var speed = (_isSlowdown) ? playerSingleton.slowMoveSpeed : playerSingleton.moveSpeed;

        // 水平入力、垂直入力からfloat3を作成
        var movementDirection = new float3(_horizontalValue, 0.0f, _verticalValue);

        // 入力がある場合は正規化
        movementDirection = (math.lengthsq(movementDirection) > 0) ?
                math.normalize(movementDirection) : movementDirection;

        // 移動量を算出
        var scaledMovement = movementDirection * speed * delta;

        // 移動を加算代入
        currentPosition += (float3)scaledMovement;

        // 移動可能範囲内に収める
        currentPosition.x = Mathf.Clamp(currentPosition.x, minMovementRange.x, maxMovementRange.x);
        currentPosition.y = Mathf.Clamp(currentPosition.y, minMovementRange.y, maxMovementRange.y);
        currentPosition.z = Mathf.Clamp(currentPosition.z, minMovementRange.z, maxMovementRange.z);

        // PLの位置を範囲内に収める
        playerTransform.Position = currentPosition;

        // PLの変更を反映
        SystemAPI.SetComponent(playerEntity, playerTransform);
    }

    /// <summary>
    /// PLの通常攻撃
    /// </summary>
    /// <param name="elapsed">経過時間</param>
    /// <param name="playerEntity">PLEntity</param>
    /// <param name="playerSingleton">PLData</param>
    /// <param name="playerTransform">PLTfm</param>
    public void PlayerShot(double elapsed, Entity playerEntity, PlayerSingletonData playerSingleton, LocalTransform playerTransform)
    {
        // 最後に射撃した時刻と現在の時間から経過時間を求め、射撃間隔を満たしていなければ切り上げる
        if (elapsed - _lastShotTime <= playerSingleton.firingInterval) { return; }

        // 最後に射撃した時刻を更新
        _lastShotTime = elapsed;

        // PLの弾を生成
        var playerBulletEntity = EntityManager.Instantiate(playerSingleton.playerBulletEntity);

        // 弾のLocalTransformを取得
        var bulletTfm = SystemAPI.GetComponent<LocalTransform>(playerBulletEntity);

        // 現在地を代入
        bulletTfm.Position = playerTransform.Position;

        // 現在の回転を代入
        bulletTfm.Rotation = playerTransform.Rotation;

        // PLとPLの弾の変更を適用
        SystemAPI.SetComponent(playerBulletEntity, bulletTfm);
        SystemAPI.SetComponent(playerEntity, playerSingleton);
    }

    /// <summary>
    /// 移動時に体を傾ける処理を開始
    /// </summary>
    /// <param name="context"></param>
    private void TiltOnMoveStart(InputAction.CallbackContext context)
    {
        // PlayerSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // シングルトンデータの取得
        var playerSingleton = SystemAPI.GetSingleton<PlayerSingletonData>();

        // PlayerSingletonDataが持つEntityを取得
        var modelEntity = playerSingleton.playerModelEntity;

        // LocalTransformを所持していなかった
        if (!SystemAPI.HasComponent<LocalTransform>(modelEntity)) { return; }

        // ModelからLocalTransformを取得
        var playerTransform = SystemAPI.GetComponent<LocalTransform>(modelEntity);

        // 1、0、-1の整数に変換
        var dir = (int)math.sign(_horizontalValue);

        // 変更を適用
        playerTransform.Rotation = quaternion.EulerXYZ(0, math.radians(playerSingleton.playerMoveTilt * dir), 0);
        SystemAPI.SetComponent(modelEntity, playerTransform);
    }

    /// <summary>
    /// 移動時に体を傾ける処理を終了
    /// </summary>
    /// <param name="context"></param>
    public void TiltOnMoveEnd(InputAction.CallbackContext context)
    {
        // PlayerSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // シングルトンデータの取得
        var playerSingleton = SystemAPI.GetSingleton<PlayerSingletonData>();

        // PlayerSingletonDataが持つEntityを取得
        var modelEntity = playerSingleton.playerModelEntity;

        // LocalTransformを所持していなかった
        if (!SystemAPI.HasComponent<LocalTransform>(modelEntity)) { return; }

        // ModelからLocalTransformを取得
        var playerTransform = SystemAPI.GetComponent<LocalTransform>(modelEntity);

        // 傾きを初期化
        playerTransform.Rotation = Quaternion.identity;

        // 変更を適用
        SystemAPI.SetComponent(modelEntity, playerTransform);
    }

    /// <summary>
    /// PLの被弾時の処理を呼び出す
    /// </summary>
    private void PlayerDamageTriggerJob()
    {
        // 更新する
        _playerSingletonLookup.Update(this);
        _healthPointLookup.Update(this);
        _dealDamageLookup.Update(this);
        _destroyableLookup.Update(this);
        _remainingPierceCountLookup.Update(this);
        _localTransformLookup.Update(this);
        _vfxCreationLookup.Update(this);

        // PLに弾が当たった時の処理を呼び出す
        var playerDamage = new PlayerDamageTriggerJob()
        {
            playerSingletonLookup = _playerSingletonLookup,
            dealDamageLookup = _dealDamageLookup,
            destroyableLookup = _destroyableLookup,
            localTransformLookup = _localTransformLookup,
            vfxCreationLookup = _vfxCreationLookup
        };

        // 前のジョブを完了する
        Dependency.Complete();

        var playerJobHandle = playerDamage.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), Dependency);

        // ジョブの依存関係を更新する
        Dependency = playerJobHandle;
    }
}
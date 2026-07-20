using UnityEngine;
using UnityEngine.InputSystem;

namespace PESDISASTER
{
    /// <summary>
    /// ハンドガンの機能を制御するクラス
    /// </summary>
    public class HandgunController : MonoBehaviour
    {
        /// <summary>
        /// リロードミニゲームのUIを管理するクラスを参照する変数
        /// </summary>
        [SerializeField]
        private ReloadMinigameManager _reloadMinigameManager;
        /// <summary>
        /// プレイヤーへの通知UIを管理するクラスを参照する変数
        /// </summary>
        [SerializeField]
        private PlayerNoticeUI_Manager _playerNoticeUI;
        /// <summary>
        /// アイテムを管理するクラスを参照する変数
        /// </summary>
        [SerializeField]
        private ItemManager _itemManager;

        /// <summary>
        /// マズルフラッシュの親パーティクルシステムを参照する変数
        /// </summary>
        [SerializeField]
        private ParticleSystem _muzzleFlashParent;
        /// <summary>
        /// マズルフラッシュの子パーティクルシステムを参照する変数
        /// </summary>
        [SerializeField]
        private ParticleSystem _muzzleFlashChild;

        /// <summary>
        /// 銃モデルにつけるマガジンの位置を参照する変数
        /// </summary>
        [SerializeField]
        private Transform _magazineTransform;

        /// <summary>
        /// ハンドガンコントローラーのインスタンスを参照する変数
        /// </summary>
        public static HandgunController Instance { get; private set; }

        /// <summary>
        /// プレイヤーのメインカメラを参照する変数
        /// </summary>
        public Camera fpsCamera;

        /// <summary>
        /// 銃口の空オブジェクトを参照する変数
        /// </summary>
        public Transform muzzleLocation;
        /// <summary>
        /// エイム時の銃の位置・回転を参照する変数
        /// </summary>
        public Transform aimTransform;
        /// <summary>
        /// 腰持ち時の銃の位置・回転を参照する変数
        /// </summary>
        public Transform hipTransform;

        /// <summary>
        /// 着弾時の火花や弾痕のプレハブを参照する変数
        /// </summary>
        public GameObject impactEffectPrefab;
        /// <summary>
        /// 空のマガジンモデルを参照する変数
        /// </summary>
        public GameObject EmptyMagazineModel;
        /// <summary>
        /// 満タンのマガジンモデルを参照する変数
        /// </summary>
        public GameObject Full_MagazineModel;
        /// <summary>
        /// 現在のマガジンモデルを参照する変数
        /// </summary>
        private GameObject _currentMagazineModel = null;

        /// <summary>
        /// ハンドガンのアニメーターを参照する変数
        /// </summary>
        public Animator HandgunAnimator;

        /// <summary>
        /// エイム時の銃の位置を参照する変数
        /// </summary>
        public Vector3 AimPosition;
        /// <summary>
        /// 通常時の銃の位置を参照する変数
        /// </summary>
        private Vector3 _hipPosition;

        /// <summary>
        /// ハンドガンのリロードイントロモーショントリガーIDを参照する変数
        /// </summary>
        public static readonly int Handgun_IntroTrigger_ID = Animator.StringToHash("OnHandgun_Intro");
        /// <summary>
        /// ハンドガンのステージ1トリガーIDを参照する変数
        /// </summary>
        public static readonly int HandgunStage1_Trigger_ID = Animator.StringToHash("OnHandgunStage1");
        /// <summary>
        /// ハンドガンのリロードアウトロモーショントリガーIDを参照する変数
        /// </summary>
        public static readonly int HandgunOutroTrigger_ID = Animator.StringToHash("OnHandgunOutro");
        /// <summary>
        /// ハンドガンのステージ2トリガーIDを参照する変数
        /// </summary>
        public static readonly int HandgunStage2_Trigger_ID = Animator.StringToHash("OnHandgunStage2");
        /// <summary>
        /// ハンドガンのステージ3トリガーIDを参照する変数
        /// </summary>
        public static readonly int HandgunStage3_Trigger_ID = Animator.StringToHash("OnHandgunStage3");
        /// <summary>
        /// ハンドガンのステージ4トリガーIDを参照する変数
        /// </summary>
        public static readonly int HandgunStage4_Trigger_ID = Animator.StringToHash("OnHandgunStage4");
        /// <summary>
        /// 射撃アニメーションのトリガー名を参照する変数
        /// </summary>
        private static readonly int _shootTrigger_ID = Animator.StringToHash("Fire");
        /// <summary>
        /// マガジンの最大装弾数を参照する変数
        /// </summary>
        public int maxClipAmmo = 10;
        /// <summary>
        /// 現在のマガジン内弾数を参照する変数
        /// </summary>
        public int currentAmmo = 10;
        /// <summary>
        /// 予備の持ち弾を参照する変数
        /// </summary>
        public int reserveAmmo = 5;

        /// <summary>
        /// 次に射撃できる時間を参照する変数
        /// </summary>
        private float nextTimeToFire = 0f;
        /// <summary>
        /// 着弾エフェクトが残る時間
        /// </summary>
        private float impactEffectDestroyLimit = 2f;
        /// <summary>
        /// 射程距離を参照する変数
        /// </summary>
        public float range = 50f;
        /// <summary>
        /// 発射レートを参照する変数
        /// </summary>
        public float fireRate = 0.3f;
        /// <summary>
        /// リロードにかかる時間を参照する変数
        /// </summary>
        public float reloadTime = 1f;
        /// <summary>
        /// 1発のダメージ量を参照する変数
        /// </summary>
        public float damage = 20f;
        /// <summary>
        /// 構えるスピードを参照する変数
        /// </summary>
        public float aimSpeed = 10f;
        /// <summary>
        /// 通常時のカメラ視野角を参照する変数
        /// </summary>
        public float normalFOV = 60f;
        /// <summary>
        /// エイム時のカメラ視野角を参照する変数
        /// </summary>
        public float aimFOV = 40f;

        /// <summary>
        /// リロード中かどうかを参照する変数
        /// </summary>
        public bool IsReloading = false;
        /// <summary>
        /// 銃を装備しているかを参照する変数
        /// </summary>
        public bool isEquipped = false;
        /// <summary>
        /// エイム中かどうかを参照する変数
        /// </summary>
        public bool IsAiming = false;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // もしインスタンスが無い場合
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // ゲーム開始時の銃の位置を「通常時の位置」として記憶しておく
            _hipPosition = transform.localPosition;

            // もしカメラが設定されている場合
            if (fpsCamera != null)
            {
                // カメラの初期FOVを設定
                fpsCamera.fieldOfView = normalFOV;
            }

            // 満タンのマガジンモデルを銃モデルに生成
            ChangeMagazine(Full_MagazineModel,null);
        }

        /// <summary>
        /// 使用可能状態にする関数
        /// </summary>
        public void EquipGun(Camera playerCamera)
        {
            fpsCamera = playerCamera;
            isEquipped = true;// 銃を装備した状態にする
        }

        /// <summary>
        /// 射撃入力を処理する関数
        /// </summary>
        public void OnShoot(InputAction.CallbackContext context)
        {
            // もし銃が装備されていない、もしくはリロード中の場合、もしくはプレイヤーが動けない場合
            if (!isEquipped || IsReloading || PlayerController.Instance.IsSleeping)
            {
                return;
            }

            // もしボタンが押された場合と、前回の射撃からfireRate以上の時間が経過している場合
            if (context.performed && Time.time >= nextTimeToFire)
            {
                // もしマガジン内に弾が残っている場合
                if (currentAmmo > 0)
                {
                    // 次に射撃できる時間を更新
                    nextTimeToFire = Time.time + fireRate;
                    // 射撃処理の関数を呼び出す
                    Shoot();
                }
                else
                {
                    // 空マガジン用SEを再生
                    AudioManager.Instance.PlaySE("NonMagazine");
                    // 弾切れ通知アニメーションをする
                    _playerNoticeUI.NoticeEmpty();
                }
            }
        }

        /// <summary>
        /// リロード入力を処理する関数
        /// </summary>
        public void OnReload(InputAction.CallbackContext context)
        {
            // もし銃が装備されていない、もしくはリロード中の場合、もしくはプレイヤーが動けない場合
            if (!isEquipped || IsReloading || PlayerController.Instance.IsSleeping)
            {
                return;
            }

            // もしボタンが押された場合と、弾が減っている場合、予備弾薬がある場合
            if (context.performed && currentAmmo < maxClipAmmo && reserveAmmo > 0)
            {
                // ミニゲーム開始し、引数に「終わった後に実行する処理」を渡す
                _reloadMinigameManager.StartMinigame((bool success) =>
                {
                    // もしミニゲームが成功した場合
                    if (success)
                    {
                        Reload();// 成功時のみ実際のリロードを開始
                    }
                    else
                    {
                        // 失敗時のガシャン！という音などをここで鳴らす

                        _playerNoticeUI.NoticeReloadFailed();// リロード失敗通知アニメーションを行う
                    }
                });
            }
        }

        /// <summary>
        /// 実際の射撃処理を行う関数
        /// </summary>
        public void Shoot()
        {
            // --- 射撃演出を行う ---
            // マガジン内の弾数を1減らす
            currentAmmo--;
            // 射撃SEを再生
            AudioManager.Instance.PlaySE("Shoot");
            // 射撃アニメーションを再生する
            HandgunAnimator.SetTrigger(_shootTrigger_ID);
            // --- マズルフラッシュの親エフェクトが光る ---
            _muzzleFlashParent.Stop();
            _muzzleFlashParent.Play();
            // --- マズルフラッシュの子エフェクトが光る ---
            _muzzleFlashChild.Stop();
            _muzzleFlashChild.Play();
            // マガジンモデルを変更
            ChangeMagazine(EmptyMagazineModel,"Hold_Item");
            // プレイヤーのリコイル画面揺れを起こす
            PlayerController.Instance.AddCameraRecoil(8.0f, 2.0f);

            // --- レイを発射 ---
            // 画面中央からRayを飛ばして当たり判定を行い参照する変数を定義
            Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            // Rayが何かに当たった情報を格納する変数を定義
            RaycastHit hit;

            // もしRayが何かに当たった場合
            if (Physics.Raycast(ray, out hit, range))
            {
                HealthManager enemy = hit.transform.GetComponent<HealthManager>();// 当たった相手にHealthManagerスクリプトがついているか確認

                // もしHealthManagerスクリプトがついている場合
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }

                ShootableLock targetLock = hit.collider.GetComponent<ShootableLock>();// 当たったオブジェクトが錠前を持っているか確認

                // もし錠前を持っている場合
                if (targetLock != null)
                {
                    targetLock.StartCoroutine(targetLock.BreakLockCoroutine());// 錠前だったら破壊処理を実行
                }

                // もし着弾エフェクトのプレハブが設定されている場合
                if (impactEffectPrefab != null)
                {
                    // 法線に合わせてエフェクトを生成
                    GameObject impactGO = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));// エフェクトを生成
                    Destroy(impactGO, impactEffectDestroyLimit);// 指定秒後に消去
                }
            }
        }

        /// <summary>
        /// リロード処理を行うコルーチン
        /// </summary>
        private void Reload()
        {
            AudioManager.Instance.PlaySE("Reload");

            // リロード完了後の弾数の計算
            int ammoNeeded = maxClipAmmo - currentAmmo;// 補充すべき弾数を計算
            int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);// 予備弾薬が足りない場合は、持っている分だけ補充

            // マガジン内の弾数と予備弾薬を更新
            currentAmmo += ammoToReload;// マガジン内の弾数を補充
            reserveAmmo -= ammoToReload;// 予備弾薬から補充した分を減らす

            _playerNoticeUI.NoticeReloadComplete();// リロード完了通知アニメーションを行う
        }

        /// <summary>
        /// 毎フレーム処理を行う関数
        /// </summary>
        private void Update()
        {
            // もし装備していない場合
            if (!isEquipped)
            {
                return;
            }

            // エイムのスムーズさを調整するためのステップ値を計算
            float step = Time.deltaTime * aimSpeed;

            // ターゲットをaimTransformかhipTransformで切り替える
            Transform target = IsAiming ? aimTransform : hipTransform;

            // もしターゲットが設定されている場合
            if (target != null)
            {
                // --- 銃本体(transform)を、ターゲットの位置・回転へ滑らかに移動させる ---
                // 位置をスムーズに補間
                transform.localPosition = Vector3.Lerp(transform.localPosition, target.localPosition, step);
                // 回転をスムーズに補間
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target.localRotation, step);
            }

            // もしカメラが設定されている場合
            if (fpsCamera != null)
            {
                // 目標FOVを決定し参照する変数を定義
                float targetFOV = IsAiming ? aimFOV : normalFOV;
                // カメラのFOVをスムーズに変化させる
                fpsCamera.fieldOfView = Mathf.Lerp(fpsCamera.fieldOfView, targetFOV, step);
            }
        }

        /// <summary>
        /// Input Systemの「Aim」アクションに紐付ける関数
        /// </summary>
        /// <param name="context"></param>
        public void OnAim(InputAction.CallbackContext context)
        {
            // もしプレイヤーが動けないか、リロード中の場合
            if (PlayerController.Instance.IsSleeping || IsReloading)
            {
                return;
            }

            // もし銃装備中にボタンが押された場合
            if (context.performed && isEquipped)
            {
                IsAiming = true;
            }
            // もし銃装備中にボタンが離された場合
            else if (context.canceled && isEquipped)
            {
                IsAiming = false;
            }
        }

        /// <summary>
        /// 指定のモーションを再生する関数
        /// </summary>
        /// <param name="trigger_ID"></param>
        public void ReloadMotion(int trigger_ID)
        {
            // 指定のモーション再生
            HandgunAnimator.SetTrigger(trigger_ID);
        }

        /// <summary>
        /// マガジンモデルを変更する関数
        /// </summary>
        /// <param name="addMagazineModel"></param>
        /// <param name="layerName"></param>
        public void ChangeMagazine(GameObject addMagazineModel,string layerName)
        {
            // 指定の追加用マガジンが空の場合
            if (addMagazineModel == null)
            {
                return;
            }

            // すでにシーンに存在する古いマガジンを削除する
            if (_currentMagazineModel != null)
            {
                Destroy(_currentMagazineModel);
            }

            // --- 銃のマガジンモデルを変更する ---
            // マガジンモデル自体を生成し、現在のマガジン変数に設定
            _currentMagazineModel = Instantiate(addMagazineModel);
            // マガジンモデルを銃モデル内マガジン座標の子にする
           _currentMagazineModel.transform.SetParent(_magazineTransform);
            // もしレイヤー名が指定されているなら
            if (layerName != null)
            {
                // 入手アイテムレイヤーをint型に変換
                int targetLayer = LayerMask.NameToLayer(layerName);
                // マガジンのレイヤーを変更
                _itemManager.SetLayerRecursively(_currentMagazineModel, targetLayer);
            }
            // マガジンのローカル座標をゼロに設定
            _currentMagazineModel.transform.localPosition = Vector3.zero;
            // マガジンのサイズを標準に設定
            _currentMagazineModel.transform.localScale = Vector3.one;
        }
    }
}
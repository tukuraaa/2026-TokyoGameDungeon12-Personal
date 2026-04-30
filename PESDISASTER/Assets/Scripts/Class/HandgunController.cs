using System.Collections;
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
        /// リロードミニゲームのUIを管理するクラスを参照する変数
        /// </summary>
        public ReloadMinigameUI_Manager reloadMinigameUI;
        /// <summary>
        /// プレイヤーへの通知UIを管理するクラスを参照する変数
        /// </summary>
        public PlayerNoticeUI_Manager playerNoticeUI;

        /// <summary>
        /// 銃のアニメーターを参照する変数
        /// </summary>
        public Animator gunAnimator;

        /// <summary>
        /// 通常時の銃の位置
        /// </summary>
        private Vector3 hipPosition;
        /// <summary>
        /// エイム時の銃の位置を参照する変数
        /// </summary>
        public Vector3 aimPosition;

        /// <summary>
        /// 射撃アニメーションのトリガー名を参照する変数
        /// </summary>
        private static readonly int gunAnimatorTrigger = Animator.StringToHash("Fire");
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
        public int reserveAmmo = 300000;

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
        private bool isReloading = false;
        /// <summary>
        /// エイム中かどうかを参照する変数
        /// </summary>
        private bool isAiming = false;
        /// <summary>
        /// 銃を装備しているかを参照する変数
        /// </summary>
        public bool isEquipped = false;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            hipPosition = transform.localPosition;// ゲーム開始時の銃の位置を「通常時の位置」として記憶しておく

            // もしカメラが設定されている場合
            if (fpsCamera != null)
            {
                fpsCamera.fieldOfView = normalFOV;// カメラの初期FOVを設定
            }
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
        public void OnFire(InputAction.CallbackContext context)
        {
            // もし銃が装備されていない、もしくはリロード中の場合
            if (!isEquipped || isReloading)
            {
                return;
            }

            // もしボタンが押された場合と、前回の射撃からfireRate以上の時間が経過している場合
            if (context.performed && Time.time >= nextTimeToFire)
            {
                // もしマガジン内に弾が残っている場合
                if (currentAmmo > 0)
                {
                    nextTimeToFire = Time.time + fireRate;// 次に射撃できる時間を更新
                    Shoot();
                }
                else
                {
                    // 弾切れの「カチッ」という音を鳴らす処理などをここに入れる

                    playerNoticeUI.StartEmpty();// 弾切れ通知アニメーションをする
                }
            }
        }

        /// <summary>
        /// リロード入力を処理する関数
        /// </summary>
        public void OnReload(InputAction.CallbackContext context)
        {
            // もし銃が装備されていない、もしくはリロード中の場合
            if (!isEquipped || isReloading)
            {
                return;
            }

            // もしボタンが押された場合と、弾が減っている場合、予備弾薬がある場合
            if (context.performed && currentAmmo < maxClipAmmo && reserveAmmo > 0)
            {
                // ミニゲーム開始し、引数に「終わった後に実行する処理」を渡す
                reloadMinigameUI.StartMinigame((bool success) => {

                    // もしミニゲームが成功した場合
                    if (success)
                    {
                        StartCoroutine(ReloadRoutine());// 成功時のみ実際のリロードを開始
                    }
                    else
                    {
                        // 失敗時のガシャン！という音などをここで鳴らす
                        
                        playerNoticeUI.StartFailed();// リロード失敗通知アニメーションを行う
                    }
                });
            }
        }

        /// <summary>
        /// 実際の射撃処理を行う関数
        /// </summary>
        public void Shoot()
        {
            currentAmmo--;// マガジン内の弾数を1減らす

            // もし銃のアニメーターが設定されている場合
            if (gunAnimator != null)
            {
                gunAnimator.SetTrigger(gunAnimatorTrigger);
            }

            // 画面中央からRayを飛ばして当たり判定を行う
            Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));// 画面中央からRayを飛ばす

            RaycastHit hit;// Rayが何かに当たった情報を格納する変数

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
        /// リロードのコルーチン
        /// </summary>
        private IEnumerator ReloadRoutine()
        {
            isReloading = true;

            // ここでリロードアニメーションや音を再生する

            yield return new WaitForSeconds(reloadTime);

            // リロード完了後の弾数の計算
            int ammoNeeded = maxClipAmmo - currentAmmo;// 補充すべき弾数を計算
            int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);// 予備弾薬が足りない場合は、持っている分だけ補充

            // マガジン内の弾数と予備弾薬を更新
            currentAmmo += ammoToReload;// マガジン内の弾数を補充
            reserveAmmo -= ammoToReload;// 予備弾薬から補充した分を減らす

            isReloading = false;

            playerNoticeUI.StartReload();// リロード完了通知アニメーションを行う
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

            float step = Time.deltaTime * aimSpeed;// エイムのスムーズさを調整するためのステップ値を計算

            Transform target = isAiming ? aimTransform : hipTransform ;// ターゲットを「aimTransform」か「hipTransform」で切り替える

            // もしターゲットが設定されている場合
            if (target != null)
            {
                // 銃本体(transform)を、ターゲットの位置・回転へ滑らかに移動させる
                transform.localPosition = Vector3.Lerp(transform.localPosition, target.localPosition, step);// 位置をスムーズに補間
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target.localRotation, step);// 回転をスムーズに補間
            }

            // もしカメラが設定されている場合
            if (fpsCamera != null)
            {
                float targetFOV = isAiming ? aimFOV : normalFOV;// 目標FOVを決定
                fpsCamera.fieldOfView = Mathf.Lerp(fpsCamera.fieldOfView, targetFOV, step);// カメラのFOVをスムーズに変化させる
            }
        }

        /// <summary>
        /// Input Systemの「Aim」アクションに紐付ける関数
        /// </summary>
        /// <param name="context"></param>
        public void OnAim(InputAction.CallbackContext context)
        {
            // もし銃装備中にボタンが押された場合
            if (context.performed && isEquipped)
            {
                isAiming = true;
            }
            // もし銃装備中にボタンが離された場合
            else if (context.canceled && isEquipped)
            {
                isAiming = false;
            }
        }
    }
}
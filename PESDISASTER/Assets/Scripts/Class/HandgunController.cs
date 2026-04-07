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
        /// 着弾時の火花や弾痕のプレハブを参照する変数
        /// </summary>
        public GameObject impactEffectPrefab;

        /// <summary>
        /// リロードミニゲームのUIを管理するクラスを参照する変数
        /// </summary>
        private ReloadMinigameUI_Manager reloadMinigameUI;

        /// <summary>
        /// 銃のアニメーターを参照する変数
        /// </summary>
        public Animator gunAnimator;

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
        public int reserveAmmo = 30;

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
        public float reloadTime = 1.5f;
        /// <summary>
        /// 1発のダメージ量を参照する変数
        /// </summary>
        public float damage = 20f;

        /// <summary>
        /// リロード中かどうかを参照する変数
        /// </summary>
        private bool isReloading = false;
        /// <summary>
        /// 銃を装備しているかを参照する変数
        /// </summary>
        private bool isEquipped = false;

        /// <summary>
        /// リロードミニゲームのUIを管理するクラスのオブジェクト名を参照する変数
        /// </summary>
        private string reloadMinigameUI_Name = "ReloadMinigameUI";


        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            reloadMinigameUI = GameObject.Find(reloadMinigameUI_Name).GetComponent<ReloadMinigameUI_Manager>();// シーン内からReloadMinigameUIを探して取得
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

                    Debug.Log("弾切れ！リロードしてください");
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
                        Debug.Log("リロード成功！");
                        StartCoroutine(ReloadRoutine());// 成功時のみ実際のリロードを開始
                    }
                    else
                    {
                        Debug.Log("リロード失敗...");

                        // 失敗時のガシャン！という音などをここで鳴らす
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
            Debug.Log($"バン！ 残弾: {currentAmmo} / {reserveAmmo}");

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
                Debug.Log(hit.transform.name + " に命中！");
               
                EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();// 当たった相手に EnemyHealth スクリプトがついているか確認

                // もし EnemyHealth スクリプトがついている場合
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
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
            Debug.Log("リロード中...");

            // ここでリロードアニメーションや音を再生する

            yield return new WaitForSeconds(reloadTime);

            // リロード完了後の弾数の計算
            int ammoNeeded = maxClipAmmo - currentAmmo;// 補充すべき弾数を計算
            int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);// 予備弾薬が足りない場合は、持っている分だけ補充

            // マガジン内の弾数と予備弾薬を更新
            currentAmmo += ammoToReload;// マガジン内の弾数を補充
            reserveAmmo -= ammoToReload;// 予備弾薬から補充した分を減らす

            isReloading = false;
            Debug.Log($"リロード完了！ 残弾: {currentAmmo} / {reserveAmmo}");
        }
    }
}
using System.Collections;
using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// 棚の状態を管理するクラス
    /// </summary>
    public class LockedShelf : MonoBehaviour, S_Interactable
    {
        /// <summary>
        /// 棚のドアのTransformを参照する変数
        /// </summary>
        public Transform doorTransform;
        /// <summary>
        /// 鍵のTransformを参照する変数
        /// </summary>
        public Transform keyTransform;

        /// <summary>
        /// 錠前のスクリプトを参照する変数
        /// </summary>
        public ShootableLock shootableLock;
        /// <summary>
        /// プレイヤー通知UIを管理するクラスを参照する変数
        /// </summary>
        public PlayerNoticeUI_Manager playerNoticeUI_Manager;
        /// <summary>
        /// プレイヤー操作を管理するクラスを参照する変数
        /// </summary>
        public PlayerController playerController;

        /// <summary>
        /// 鍵がかかっているかどうかを参照する変数
        /// </summary>
        private bool isLocked = true;
        /// <summary>
        /// 開けたかどうかを参照する変数
        /// </summary>
        private bool isOpen = false;
        /// <summary>
        /// 最初のインタラクトかどうかを参照する変数
        /// </summary>
        public bool isFirst = true;

        /// <summary>
        /// 棚を開けるときのX軸移動量を参照する変数
        /// </summary>
        private float transformDoorPositionX_Value = -0.501f;
        /// <summary>
        /// 棚を開けるアニメーションの時間を参照する変数
        /// </summary>
        private float openDuration = 1f;
        /// <summary>
        /// 棚を開けるときのX軸移動量を参照する変数
        /// </summary>
        private float transformKeyPositionX_Value = -0.501f;
        /// <summary>
        /// チュートリアルアニメーションの時間を参照する変数
        /// </summary>
        private float tutorialAnimTime = 3.1f;
        /// <summary>
        /// 通知アニメーションの時間を参照する変数
        /// </summary>
        private float noticeAnimTime = 2.1f;

        /// <summary>
        /// 錠前が破壊された時に呼ばれる関数
        /// </summary>
        public void Unlock()
        {
            isLocked = false;
        }

        /// <summary>
        /// プレイヤーがアクセスした時の処理の関数
        /// </summary>
        public void OpenShelf()
        {
            // もし棚のドアが鍵がかかっている場合
            if (isLocked)
            {
                playerNoticeUI_Manager.StartLocked();// ロック中なのを通知する

                // もし初めて調べた場合
                if (isFirst)
                {
                    StartCoroutine(OpenShelfTutorialCoroutine());// 棚の開け方チュートリアルを開始する
                }

                return;
            }

            // もし棚のドアが開いていないかつ、錠前が破壊されている場合
            if (!isOpen && shootableLock.isBroken)
            {
                isOpen = true;

                this.gameObject.GetComponent<Collider>().enabled = false;// オブジェクトのコライダーを無効する（インタラクトできないようにする）

                StartCoroutine(OpenShelfCoroutine(doorTransform.position + new Vector3(transformDoorPositionX_Value, 0, 0), keyTransform.position + new Vector3(transformKeyPositionX_Value, 0, 0), openDuration));// ドアを開けるアニメーションを開始
            }
        }

        /// <summary>
        /// 棚を開けるアニメーションを行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator OpenShelfCoroutine(Vector3 targetDoorPosition, Vector3 targetKeyPosition, float duration)
        {
            Vector3 startDoorPosition = doorTransform.position; // 開始位置
            Vector3 startKeyPosition = keyTransform.position; // 開始位置
            float elapsed = 0f; // 経過時間

            // durationの間、毎フレーム位置を更新していく
            while (elapsed < duration)
            {
                float t = elapsed / duration;// 経過時間をもとにLerpの割合（0～1）を計算

                doorTransform.position = Vector3.Lerp(startDoorPosition, targetDoorPosition, t);// Lerpで位置を補間
                keyTransform.position = Vector3.Lerp(startKeyPosition, targetKeyPosition, t);// Lerpで位置を補間

                elapsed += Time.deltaTime;// 経過時間を更新

                yield return null;
            }

            doorTransform.position = targetDoorPosition;// 最後に確実に目的地に配置
            keyTransform.position = targetKeyPosition;// 最後に確実に目的地に配置
        }

        /// <summary>
        /// 棚を開ける方法を記すアニメーションを行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator OpenShelfTutorialCoroutine()
        {
            playerController.enabled = false;// プレイヤーを操作させない
            isFirst = false;
            yield return new WaitForSeconds(noticeAnimTime);// 通知UIを表示する時間分待機
            playerNoticeUI_Manager.StartOpenTutorial();// 棚の開け方チュートリアルを開始する
            yield return new WaitForSeconds(tutorialAnimTime);
            playerController.enabled = true;// プレイヤーを操作できるようにする
        }
    }
}
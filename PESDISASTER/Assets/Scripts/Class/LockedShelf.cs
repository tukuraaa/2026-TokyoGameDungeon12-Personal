using System.Collections;
using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// 棚の状態を管理するクラス
    /// </summary>
    public class LockedShelf : MonoBehaviour,S_Interactable
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
        /// 鍵がかかっているかどうかを管理する変数
        /// </summary>
        private bool isLocked = true;
        /// <summary>
        /// 開けたかどうかを管理する変数
        /// </summary>
        private bool isOpen = false;

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
        /// 錠前が破壊された時に呼ばれる関数
        /// </summary>
        public void Unlock()
        {
            isLocked = false;
            Debug.Log("棚の鍵が壊された！");
        }

        /// <summary>
        /// プレイヤーがアクセスした時の処理の関数
        /// </summary>
        public void OpenShelf()
        {
            // もし棚のドアが鍵がかかっている場合
            if (isLocked)
            {
                Debug.Log("鍵がかかっている。銃で破壊できそうだ。");

                // ここで「ガチャガチャ」という音を鳴らすとリアルです

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
    }
}
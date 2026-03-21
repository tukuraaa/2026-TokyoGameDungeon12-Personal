using UnityEngine;
using System.Collections;

namespace PESDISASTER
{
    /// <summary>
    /// アイテムを拾うためのクラス
    /// </summary>
    public class PickupItem : MonoBehaviour, IInteractable
    {
        /// <summary>
        /// アイテムの名前を参照する変数
        /// </summary>
        public string itemName = "ハンドガン";
        /// <summary>
        /// カメラの名前を参照する変数
        /// </summary>
        public string cameraName = "MainCamera";
        /// <summary>
        /// 右手元の名前を参照する変数
        /// </summary>
        public string holdName = "HoldPosition";
        /// <summary>
        /// 特定のレイヤーの名前を参照する変数
        /// </summary>
        public string layerName = "Hold_Item";

        /// <summary>
        /// アイテムを拾ったか否かのフラグを参照する変数
        /// </summary>
        private bool isPickedUp = false;

        /// <summary>
        /// イージング時に調整するための値を参照する変数
        /// </summary>
        private float easingNumber = 3f;
        /// <summary>
        /// アイテムが手元に移動するまでの時間を参照する変数
        /// </summary>
        public float moveDuration = 0.5f;

        /// <summary>
        /// アイテムの物理演算を参照する変数
        /// </summary>
        private Rigidbody itemRigidbody = null;

        /// <summary>
        /// アイテムのコライダーを参照する変数
        /// </summary>
        private Collider itemCollider= null;

        /// <summary>
        /// カメラの位置を参照する変数
        /// </summary>
        private Transform cameraTransform = null;
        /// <summary>
        /// プレイヤー右手元の位置を参照する変数
        /// </summary>
           private Transform holdPosition = null;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            itemRigidbody = GetComponent<Rigidbody>();
            itemCollider = GetComponent<Collider>();
            cameraTransform = GameObject.Find(cameraName).GetComponent<Transform>();
            holdPosition = GameObject.Find(holdName).GetComponent<Transform>();
        }

        /// <summary>
        /// 拾ったアイテムの名前を表示する関数
        /// </summary>
        /// <returns></returns>
        public string GetInteractText()
        {
            return $"{itemName} を拾う";// 拾ったアイテムの名前を表示する
        }

        /// <summary>
        /// プレイヤーのRaycast等のイベントから呼ばれるメソッド
        /// </summary>
        /// <param name="cameraTransform">メインカメラのTransform</param>
        /// <param name="holdPosition">手元の目標位置のTransform</param>
        public void Pickup()
        {
            // もし拾っている場合
            if (isPickedUp)
            {
                return;
            }

            isPickedUp = true;// 拾ったフラグをオン
            
            // RigidBodyがついている場合
            if (itemRigidbody != null)
            {
             // 物理演算を無効化する
                itemRigidbody.isKinematic = true;
                itemRigidbody.useGravity = false;
            }

            // コライダーがついている場合
            if (itemCollider != null)
            {
                itemCollider.enabled = false;// コライダーを無効化する
            }

            transform.SetParent(cameraTransform);// アイテムをカメラの子オブジェクトにする

            gameObject.layer = LayerMask.NameToLayer(layerName);// オブジェクトのレイヤーを変更

            StartCoroutine(MoveToHoldPosition(holdPosition));// 手元の位置へ滑らかに移動させるコルーチンを開始
        }

        /// <summary>
        /// 手元の位置へ滑らかに移動させるコルーチン
        /// </summary>
        /// <param name="targetHoldPosition"></param>
        /// <returns></returns>
        private IEnumerator MoveToHoldPosition(Transform targetHoldPosition)
        {
            // 移動開始前の位置と回転を記憶
            Vector3 startPosition = transform.position;
            Quaternion startRotation = transform.rotation;

            float elapsedTime = 0f;// アイテムを拾う経過時間を参照する変数

            // アイテムが手元に移動するまでの時間がアイテムを拾う経過時間より長い間はループ
            while (elapsedTime < moveDuration)
            {
                float time = elapsedTime / moveDuration;// 0～1の割合を計算

                time = time * time * (easingNumber - easingNumber-- * time);// より自然な動きにするためのイージング

                // Lerpで滑らかに補間
                transform.position = Vector3.Lerp(startPosition, targetHoldPosition.position, time);
                transform.rotation = Quaternion.Lerp(startRotation, targetHoldPosition.rotation, time);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 最後にズレをなくすため、目標と完全に一致させる
            transform.position = targetHoldPosition.position;
            transform.rotation = targetHoldPosition.rotation;
        }
    }
}
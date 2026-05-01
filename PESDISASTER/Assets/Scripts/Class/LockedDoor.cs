using UnityEngine;
using System.Collections;

namespace PESDISASTER
{
    /// <summary>
    /// ドアのロックを管理するクラス
    /// </summary>
    public class LockedDoor : MonoBehaviour, D_Interactable
    {
        /// <summary>
        /// プレイヤーの左手位置を参照する変数
        /// </summary>
        public Transform leftHandPosition;
        /// <summary>
        /// 動かすドアのモデルのTransformを参照する変数
        /// </summary>
        public Transform doorTransform;

        /// <summary>
        /// プレイヤー通知UIを管理するクラスを参照する変数
        /// </summary>
        public PlayerNoticeUI_Manager playerNoticeUI_Manager;

        /// <summary>
        /// 開けるのに必要な鍵のIDを参照する変数
        /// </summary>
        public string requiredKeyID = "BedroomKey";

        /// <summary>
        /// 開いているかどうかのフラグを参照する変数
        /// </summary>
        private bool isOpen = false;

        /// <summary>
        /// ドアを開けるアニメーションの時間を参照する変数
        /// </summary>
        private float openDuration = 1f;
        /// <summary>
        /// ドアを開けるときのY軸回転量を参照する変数
        /// </summary>
        private float transformDoorRotationY_Value = 9.3f;

        /// <summary>
        /// プレイヤーがドアにアクセスした時に呼ばれる関数
        /// </summary>
        public void Interact()
        {
            // もしすでにドアが開いている場合
            if (isOpen)
            {
                return;
            }

            Key_Item heldKey = leftHandPosition.parent.GetComponentInChildren<Key_Item>();// プレイヤーの左手位置の親オブジェクトからKey_Itemコンポーネントを持つ子オブジェクトを探す

            // もし鍵を持っていて、かつその鍵のIDが必要な鍵のIDと一致している場合
            if (heldKey != null && heldKey.keyID == requiredKeyID)
            {
                OpenDoor();

                Destroy(heldKey.gameObject);
            }
            else
            {
                playerNoticeUI_Manager.StartLocked();// ロック中なのを通知する
            }
        }

        /// <summary>
        /// ドアを開ける処理の関数
        /// </summary>
        public void OpenDoor()
        {
            isOpen = true;

            this.gameObject.GetComponent<Collider>().enabled = false;// オブジェクトのコライダーを無効する（インタラクトできないようにする）

            StartCoroutine(OpenDoorCoroutine(Quaternion.Euler(0, transformDoorRotationY_Value, 0), openDuration));// ドアを開けるアニメーションを開始
        }

        /// <summary>
        /// ドアを開けるアニメーションを行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator OpenDoorCoroutine(Quaternion targetDoorRotation, float duration)
        {
            Quaternion startDoorRotation = doorTransform.localRotation; // 開始回転
            float elapsed = 0f; // 経過時間

            // durationの間、毎フレーム回転を更新していく
            while (elapsed < duration)
            {
                float t = elapsed / duration;// 経過時間をもとにLerpの割合（0～1）を計算

                doorTransform.localRotation = Quaternion.Lerp(startDoorRotation, targetDoorRotation, t);// Lerpで回転を補間

                elapsed += Time.deltaTime;// 経過時間を更新

                yield return null;
            }

            doorTransform.localRotation = targetDoorRotation;// 最後に確実に目的地に配置
        }
    }
}
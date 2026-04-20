using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// ドアのロックを管理するクラス
    /// </summary>
    public class LockedDoor : MonoBehaviour
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
        /// 開けるのに必要な鍵のIDを参照する変数
        /// </summary>
        public string requiredKeyID = "BedroomKey";

        /// <summary>
        /// 開いているかどうかのフラグを参照する変数
        /// </summary>
        private bool isOpen = false;

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

            Key_Item heldKey = leftHandPosition.GetComponentInChildren<Key_Item>();// 左手の子オブジェクトの中から、KeyItemスクリプトを探す

            // もし鍵を持っていて、かつその鍵のIDが必要な鍵のIDと一致している場合
            if (heldKey != null && heldKey.keyID == requiredKeyID)
            {
                OpenDoor();

                Destroy(heldKey.gameObject);
            }
            else
            {
                Debug.Log("鍵がかかっている。または正しい鍵を持っていない。");

                // ガチャガチャというSEを鳴らすと良いです
            }
        }

        /// <summary>
        /// ドアを開ける処理の関数
        /// </summary>
        public void OpenDoor()
        {
            isOpen = true;
            doorTransform.localRotation = Quaternion.Euler(0, 4, 0);// ドアを開ける
        }
    }
}
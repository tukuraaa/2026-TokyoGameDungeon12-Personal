using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

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

            // 現在、プログラムがチェックしている親オブジェクトの名前を表示
            Debug.Log($"探索中の親オブジェクト: {leftHandPosition.parent.name}");

            Key_Item heldKey = leftHandPosition.parent.GetComponentInChildren<Key_Item>();// プレイヤーの左手位置の親オブジェクトからKey_Itemコンポーネントを持つ子オブジェクトを探す

            if (heldKey == null)
            {
                Debug.Log("結果: 手元に KeyItem スクリプトを持つオブジェクトが見つかりませんでした。");
            }
            else
            {
                Debug.Log($"結果: 鍵を発見！ IDは {heldKey.keyID} です。");
            }

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
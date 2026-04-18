using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// 棚の状態を管理するクラス
    /// </summary>
    public class LockedShelf : MonoBehaviour
    {
        /// <summary>
        /// 棚のドアのTransformを参照する変数
        /// </summary>
        public Transform doorTransform;

        /// <summary>
        /// 鍵がかかっているかどうかを管理する変数
        /// </summary>
        private bool isLocked = true;
        /// <summary>
        /// 開けたかどうかを管理する変数
        /// </summary>
        private bool isOpen = false;

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

            // もし棚のドアが開いていない場合
            if (!isOpen)
            {
                isOpen = true;

                doorTransform.localRotation = Quaternion.Euler(0, 90, 0);// 扉を開ける処理
                Debug.Log("棚を開けた！");
            }
        }
    }
}
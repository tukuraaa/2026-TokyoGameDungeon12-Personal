using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// ハンドガンのアニメーションイベントの受け皿クラス（エラー防止）
    /// </summary>
    public class HandgunAnimControllerSaucer : MonoBehaviour
    {
        /// <summary>
        /// アニメーションイベント'Shoot'の受け皿の関数
        /// </summary>
        public void Shoot()
        {
            // 受け皿なので空でよい
        }

        /// <summary>
        /// アニメーションイベント'CasingRelease'の受け皿の関数
        /// </summary>
        public void CasingRelease()
        {
            // 受け皿なので空でよい
        }
    }
}
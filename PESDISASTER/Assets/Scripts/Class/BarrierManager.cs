using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// 透明バリアの管理を行うクラス
    /// </summary>
    public class BarrierManager : MonoBehaviour
    {
        /// <summary>
        /// 衝突時のイベントを管理する関数
        /// </summary>
        /// <param name="collider"></param>
        private void OnTriggerEnter (Collider collider)
        {
            // もし衝突してきた相手のタグが"Player"だった場合
            if (collider.CompareTag("Player"))
            {
                // 気になるものがある場合の攻略ナビを表示
                PlayerNoticeUIManager.Instance.NoticeAction(PlayerNoticeUIManager.Instance.SomethingTextIDName);
            }
        }

        /// <summary>
        /// バリアを削除する関数
        /// </summary>
        public void DestroyBarrier()
        {
            Destroy(gameObject);
        }
    }
}
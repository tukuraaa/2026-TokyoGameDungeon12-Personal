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
                // 攻略ナビを表示
                PlayerNoticeUI_Manager.Instance.NoticeNavigate();
            }
        }

        /// <summary>
        /// バリアを削除する関数
        /// </summary>
        public void DestroyBarrier()
        {
            // もしチュートリアルが終了していないか、もしくはメモを初めて読んでいない場合
            if (!PlayerControllerUI_Manager.Instance.IsDefaultTutorialEnd|| !PaperManager.Instance.IsFirstRead)
            {
                return;
            }

            Destroy(gameObject);
        }
    }
}
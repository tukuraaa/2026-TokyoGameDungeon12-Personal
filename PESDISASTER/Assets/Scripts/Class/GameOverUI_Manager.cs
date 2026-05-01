using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// ゲームオーバーUIを管理するクラス
    /// </summary>
    public class GameOverUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            Hide();
        }

        /// <summary>
        /// UIを非表示にする関数
        /// </summary>
        private void Hide()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// UIを表示にする関数
        /// </summary>
        public void Show()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
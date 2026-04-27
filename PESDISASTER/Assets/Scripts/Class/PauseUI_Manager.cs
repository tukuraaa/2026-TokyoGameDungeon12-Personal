using UnityEngine;
using UnityEngine.UI;

namespace PESDISASTER
{
    /// <summary>
    /// ポーズUIを管理するクラス
    /// </summary>
    public class PauseUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        void Start()
        {
            Hide();
        }

        /// <summary>
        /// UIを非表示にする関数
        /// </summary>
        public void Hide()
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
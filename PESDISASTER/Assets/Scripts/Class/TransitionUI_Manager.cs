using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// 遷移用UIを管理するクラス
    /// </summary>
    public class TransitionUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
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
        /// UIを表示する関数
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

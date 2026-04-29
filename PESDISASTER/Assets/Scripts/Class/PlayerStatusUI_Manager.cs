using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// プレイヤーステータスUIを管理するクラス
    /// </summary>
    public class PlayerStatusUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator animator;

        /// <summary>
        /// アニメーターのUI表示トリガーを参照する変数
        /// </summary>
        public static readonly int showTrigger = Animator.StringToHash("OnShow");

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            animator = GetComponent<Animator>();
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
        private void Show()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// UI表示を開始する関数
        /// </summary>
        public void StartShow()
        {
            Show();
            animator.SetTrigger(showTrigger);// アニメーターのUI表示トリガーを発動
        }
    }
}
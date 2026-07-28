using UnityEngine;
using UnityEngine.UI;

namespace PESDISASTER
{
    /// <summary>
    /// メモUIを管理するクラス
    /// </summary>
    public class PaperUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// 戻るボタンを参照する変数
        /// </summary>
        public Button BackButton;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        public Animator PaperUI_Animator;

        /// <summary>
        /// アニメーターのメモイベントイントロトリガーを参照する変数
        /// </summary>
        public static readonly int Paper_Intro_ID = Animator.StringToHash("OnPaper_Intro");
        /// <summary>
        /// アニメーターのメモイベントアウトロトリガーを参照する変数
        /// </summary>
        public static readonly int PaperOutro_ID = Animator.StringToHash("OnPaperOutro");

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // 最初はUIを隠す
            ShowHide(false);
        }

        /// <summary>
        /// UIを表示非表示にする関数
        /// </summary>
        public void ShowHide(bool isActive)
        {
            // 子オブジェクトを全てチェック
            foreach (Transform child in transform)
            {
                // 子オブジェクトを表示、もしくは非表示
                child.gameObject.SetActive(isActive);
            }
        }
    }
}
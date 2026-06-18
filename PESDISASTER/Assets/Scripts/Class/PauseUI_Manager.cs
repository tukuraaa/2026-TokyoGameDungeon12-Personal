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
        /// 続行ボタンを参照する変数
        /// </summary>
        [SerializeField]
        private Button _continueButton;
        /// <summary>
        /// リトライボタンを参照する変数
        /// </summary>
        [SerializeField]
        private Button _restartButton;
        /// <summary>
        /// ポーズのタイトルボタンを参照する変数
        /// </summary>
        [SerializeField]
        private Button _titleButton;

        /// <summary>
        /// タイトルシーン名を参照する変数
        /// </summary>
        private string titleSceneName = "Title";

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // --- クリックイベントにリスナーを追加 ---
            // 続行ボタンがクリックされたとき、Pause関数を呼び出すように設定
            _continueButton.onClick.AddListener(StageManager.Instance.Pause);
            // リスタートボタンがクリックされたとき、Retry関数を呼び出すように設定
            _restartButton.onClick.AddListener(StageManager.Instance.RestartGame);
            // タイトルボタンがクリックされたとき、MoveScene関数を呼び出すように設定
            _titleButton.onClick.AddListener(() => StageManager.Instance.MoveScene(titleSceneName));

            Hide();
        }

        /// <summary>
        /// UIを非表示にする関数
        /// </summary>
        public void Hide()
        {
            // 子オブジェクトを全てチェック
            foreach (Transform child in transform)
            {
                // 子オブジェクトを非表示
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// UIを表示にする関数
        /// </summary>
        public void Show()
        {
            // 子オブジェクトを全てチェック
            foreach (Transform child in transform)
            {
                // 子オブジェクトを表示
                child.gameObject.SetActive(true);
            }
        }
    }
}
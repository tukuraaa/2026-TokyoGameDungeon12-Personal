using System;
using UnityEngine;
using UnityEngine.UI;

namespace PESDISASTER
{
    /// <summary>
    /// ポーズ機能の管理クラス
    /// </summary>
    public class PauseManager : MonoBehaviour, IUIViewer
    {
        /// <summary>
        /// 読み物UI管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private PauseUIManager _pauseUIManager;

        /// <summary>
        /// 続行ボタンを参照する変数
        /// </summary>
        [SerializeField]
        private Button _resumeButton;
        /// <summary>
        /// リトライボタンを参照する変数
        /// </summary>
        [SerializeField]
        private Button _retryButton;
        /// <summary>
        /// タイトルボタンを参照する変数
        /// </summary>
        [SerializeField]
        private Button _titleButton;

        /// <summary>
        /// UIを表示するイベントを行う変数
        /// </summary>
        public event Action<Transform, bool> OnSetActiveUI;

        /// <summary>
        /// プレイヤー管理インターフェースソース
        /// </summary>
        private IPlayerSystem _playerSystemSource;

        /// <summary>
        /// ポーズ中かどうかを示すフラグを参照する変数
        /// </summary>
        private bool _isPausing = false;

        /// <summary>
        /// ポーズSE名を参照する変数
        /// </summary>
        private string _pauseSEName = "Pause";
        /// <summary>
        /// タイトルシーン名
        /// </summary>
        private string _titleSceneName = "Title";

        /// <summary>
        /// 時間を動かすための値
        /// </summary>
        private int _canMoveTimeValue = 1;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // --- クリックイベントにリスナーを追加 ---
            // 続行ボタンがクリックされたとき、Pause関数を呼び出すように設定
            _resumeButton.onClick.AddListener(Pause);
            // リスタートボタンがクリックされたとき、Retry関数を呼び出すように設定
            _retryButton.onClick.AddListener(StageManager.Instance.RestartGame);
            // タイトルボタンがクリックされたとき、MoveScene関数を呼び出すように設定
            _titleButton.onClick.AddListener(() => StageManager.Instance.MoveScene(_titleSceneName));

            _pauseUIManager.Init(this);
        }

        /// <summary>
        /// イベント登録の初期設定を行う関数
        /// </summary>
        /// <param name="source">インターフェースのソース</param>
        public void Init(IPlayerSystem source)
        {
            _playerSystemSource = source;
            _playerSystemSource.OnInvokeSystem += Pause;
        }

        /// <summary>
        /// ポーズの処理を行う関数
        /// </summary>
        public void Pause()
        {
            AudioManager.Instance.PlaySE(_pauseSEName);

            // もしポーズしていない場合
            if (!_isPausing)
            {
                Time.timeScale = 0f;
                PlayerController.Instance.IsCanMove = false;
                OnSetActiveUI?.Invoke(_pauseUIManager.transform, true);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                _isPausing = true;
            }
            else
            {
                Time.timeScale = _canMoveTimeValue;
                PlayerController.Instance.IsCanMove = true;
                OnSetActiveUI?.Invoke(_pauseUIManager.transform, false);

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                _isPausing = false;
            }
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PESDISASTER
{
    /// <summary>
    /// リロードミニゲームのUIを管理するクラス
    /// </summary>
    public class ReloadMinigameUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// キーを表示する画像を参照する変数
        /// </summary>
        public Image PromptImage;

        /// <summary>
        /// マウス右クリックUIを参照する変数
        /// </summary>
        public Sprite RightClickSprite;
        /// <summary>
        /// TキーUIを参照する変数
        /// </summary>
        public Sprite T_KeySprite;
        /// <summary>
        /// RキーUIを参照する変数
        /// </summary>
        public Sprite R_KeySprite;
        /// <summary>
        /// マウスドラッグUIを参照する変数
        /// </summary>
        public Sprite DragDownSprite;

        /// <summary>
        /// 成否を伝えるためのコールバックを参照する変数
        /// </summary>
        private Action<bool> _onComplete;

        /// <summary>
        /// マウスドラッグの開始位置を参照する変数
        /// </summary>
        private Vector2 _dragStartPosition;

        /// <summary>
        /// アクションが有効かどうかを示すフラグを参照する変数
        /// </summary>
        private bool _isActive = false;
        /// <summary>
        /// マウスドラッグ中かどうかを示すフラグを参照する変数
        /// </summary>
        private bool _isDragging = false;

        /// <summary>
        /// マウスドラッグ距離のしきい値を参照する変数
        /// </summary>
        public float DragDistanceThreshold = 100f;

        /// <summary>
        /// リロードのステップを管理する列挙型
        /// </summary>
        private enum ReloadStep
        {
            RightClick,
            PressT,
            PressR,
            DragDown
        }
        /// <summary>
        /// 現在のステップ状態を参照する変数
        /// </summary>
        private ReloadStep _currentStep;

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
        private void Hide()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// リロードミニゲームを開始する関数
        /// </summary>
        /// <param name="callback"></param>
        public void StartMinigame(Action<bool> callback)
        {
            // もしすでにミニゲームがアクティブの場合
            if (_isActive)
            {
                return;
            }

            _onComplete = callback;// コールバックを保存
            _isActive = true;
            Show();
        }

        /// <summary>
        /// 毎フレーム処理を行う関数
        /// </summary>
        private void Update()
        {
            // もしミニゲームがアクティブでない場合
            if (!_isActive)
            {
                return;
            }
        }

        /// <summary>
        /// 成功または失敗を処理してミニゲームを終了する関数
        /// </summary>
        /// <param name="success"></param>
        private void Finish(bool success)
        {
            _isActive = false;
            Hide();
            _onComplete?.Invoke(success);// ハンドガン側に成否を伝える
        }

        /// <summary>
        /// UIを表示する関数
        /// </summary>
        private void Show()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
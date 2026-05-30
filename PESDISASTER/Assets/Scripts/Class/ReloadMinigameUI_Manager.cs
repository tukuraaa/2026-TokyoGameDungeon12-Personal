using UnityEngine;
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
        [SerializeField]
        private Image _prompt_Image;

        /// <summary>
        /// マウス右クリックUIを参照する変数
        /// </summary>
        [SerializeField]
        private Sprite _rightClickSprite;
        /// <summary>
        /// TキーUIを参照する変数
        /// </summary>
        [SerializeField]
        private Sprite _tKeySprite;
        /// <summary>
        /// RキーUIを参照する変数
        /// </summary>
        [SerializeField]
        private Sprite _rKeySprite;
        /// <summary>
        /// マウスドラッグUIを参照する変数
        /// </summary>
        [SerializeField]
        private Sprite _dragDownSprite;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // UIを隠す
            Hide();
        }

        /// <summary>
        /// UIを非表示にする関数
        /// </summary>
        public void Hide()
        {
            // 子オブジェクトを全てチェック
            foreach (Transform _child in transform)
            {
                // 子オブジェクトを非表示
                _child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// UIを表示する関数
        /// </summary>
        public void Show()
        {
            // 子オブジェクトを全てチェック
            foreach (Transform _child in transform)
            {
                // 子オブジェクトを表示
                _child.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 現在のステップに合わせてUI画像を更新する関数
        /// </summary>
        public void UpdateUI(string _control_Name)
        {
            // もし右クリックの操作だった場合
            if (_control_Name == "RightClick")
            {
                // 対応する画像に変更
                _prompt_Image.sprite = _rightClickSprite;
            }

            // もしTキークリックの操作だった場合
            if (_control_Name == "PressT")
            {
                // 対応する画像に変更
                _prompt_Image.sprite = _tKeySprite;
            }

            // もしRキークリックの操作だった場合
            if (_control_Name == "PressR")
            {
                // 対応する画像に変更
                _prompt_Image.sprite = _rKeySprite;
            }

            // もしマウス下ドラッグの操作だった場合
            if (_control_Name == "DragDown")
            {
                // 対応する画像に変更
                _prompt_Image.sprite = _dragDownSprite;
            }

            return;
        }
    }
}
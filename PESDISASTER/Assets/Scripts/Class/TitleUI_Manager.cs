using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PESDISASTER
{
    /// <summary>
    /// 遷移用UIを管理するクラス
    /// </summary>
    public class TitleUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// スタートボタンイベントトリガーを参照する変数
        /// </summary>
        [SerializeField]
        private EventTrigger _startEventTrigger;
        /// <summary>
        /// ゲーム終了ボタンイベントトリガーを参照する変数
        /// </summary>
        [SerializeField]
        private EventTrigger _exitEventTrigger;
        /// <summary>
        /// 言語変更ボタンイベントトリガーを参照する変数配列
        /// </summary>
        [SerializeField]
        private EventTrigger[] _changeLanguageEventTrigger;

        /// <summary>
        /// スタートボタンを参照する変数
        /// </summary>
        public Button StartButton;
        /// <summary>
        /// ゲーム終了ボタンを参照する変数
        /// </summary>
        public Button ExitButton;
        /// <summary>
        /// 言語変更ボタンを参照する変数配列
        /// </summary>
        public Button[] ChangeLanguageButton;

        /// <summary>
        /// ボタン・イベントトリガーなどのアクセスを管理する関数
        /// </summary>
        /// <param name="isEnabled"></param>
        public void ChangeEnabled(bool isEnabled)
        {
            // --- ボタン・ボタンイベントのアクセスを管理する ---
            // スタートボタンのオンオフを管理する
            StartButton.enabled = isEnabled;
            // スタートボタンイベントトリガーのオンオフを管理する
            _startEventTrigger.enabled = isEnabled;
            // 言語変更ボタンの配列内のすべてのボタンを順番に処理
            foreach (EventTrigger eventTrigger in _changeLanguageEventTrigger)
            {
                // 言語変更ボタンイベントトリガーのオンオフを管理する
                eventTrigger.enabled = isEnabled;
            }
            // ゲーム終了ボタンのオンオフを管理する
            ExitButton.enabled = isEnabled;
            // ゲーム終了ボタンイベントトリガーのオンオフを管理する
            _exitEventTrigger.enabled = isEnabled;
            // 言語変更ボタンの配列内のすべてのボタンを順番に処理
            foreach (Button button in ChangeLanguageButton)
            {
                // 言語変更ボタンのオンオフを管理する
                button.enabled = isEnabled;
            }
        }
    }
}
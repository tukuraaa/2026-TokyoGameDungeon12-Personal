using UnityEngine;
using UnityEngine.EventSystems;

namespace PESDISASTER
{
    /// <summary>
    /// ローカライズされたテキスト表示を管理するクラス
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーコントローラーのインスタンスを参照する変数
        /// </summary>
        public static LocalizationManager Instance { get; private set; }

        /// <summary>
        /// 初期言語の名前を参照する変数
        /// </summary>
        public string CurrentLanguage = "English";

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // もしシングルトンではない場合
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 言語を変更する関数
        /// </summary>
        /// <param name="languageCode"></param>
        public void ChangeLanguage(string languageCode)
        {
            // 初期の言語に指定の言語コードを代入する
            CurrentLanguage = languageCode;

            // シーン内のすべてのLocalizedTextDisplayを見つけ出し参照する変数を定義
            var findDisplays = FindObjectsByType<LocalizedTextDisplay>(FindObjectsSortMode.None);

            // シーン内のすべてのLocalizedTextDisplayをサーチ
            foreach (var display in findDisplays)
            {
                // サーチしたLocalizedTextDisplayを更新する
                display.UpdateText();
            }

            // ボタン選択を解除
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
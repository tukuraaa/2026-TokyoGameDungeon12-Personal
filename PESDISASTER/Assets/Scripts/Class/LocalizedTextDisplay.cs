using UnityEngine;
using TMPro;

namespace PESDISASTER
{
    /// <summary>
    /// テキストをローカライズして表示するためのクラス
    /// </summary>
    public class LocalizedTextDisplay : MonoBehaviour
    {
        /// <summary>
        /// インスペクターで言語テーブルを設定するための変数
        /// </summary>
        [SerializeField]
        private LanguageTable _languageTable;

        /// <summary>
        /// TextMeshProUGUIコンポーネントを参照する変数
        /// </summary>
        private TextMeshProUGUI _textComponent;

        /// <summary>
        /// インスペクターでテキストキーを設定するための変数
        /// </summary>
        public string TextKey;
        /// <summary>
        /// 現在の言語ID名を参照する変数
        /// </summary>
        private string _currentLanguage;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();

            // 初期表示の更新
            UpdateText();
        }

        /// <summary>
        /// テキスト表示を更新する関数
        /// </summary>
        public void UpdateText()
        {
            // --- Managerから現在の言語設定を取得して反映 -------------------------------
            _currentLanguage = LocalizationManager.Instance.CurrentLanguage;
            _textComponent.text = _languageTable.GetText(TextKey, _currentLanguage);
            // ---------------------------------------------------------------------------
        }
    }
}
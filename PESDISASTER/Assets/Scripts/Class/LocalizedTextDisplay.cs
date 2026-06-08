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
        /// インスペクターでテキストキーを設定するための変数
        /// </summary>
        [SerializeField]
        private string _textKey;

        /// <summary>
        /// TextMeshProUGUIコンポーネントを参照する変数
        /// </summary>
        private TextMeshProUGUI _textComponent;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // TextMeshProUGUIコンポーネントの取得
            _textComponent = GetComponent<TextMeshProUGUI>();

            // 初期表示の更新
            UpdateText();
        }

        /// <summary>
        /// テキスト表示を更新する関数
        /// </summary>
        public void UpdateText()
        {
            // Managerから現在の言語設定を取得して反映（後述のManager作成後に修正）
            string currentLang = LocalizationManager.Instance.CurrentLanguage;
            _textComponent.text = _languageTable.GetText(_textKey, currentLang);
        }
    }
}
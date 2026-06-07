using UnityEngine;
using TMPro;

namespace PESDISASTER
{
    public class LocalizedTextDisplay : MonoBehaviour
    {
        [SerializeField] private string textKey; // Inspectorでキーを入力（例: "Start_Button"）
        [SerializeField] private LanguageTable table;
        private TextMeshProUGUI textComponent;

        void Start()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            UpdateText();
        }

        public void UpdateText()
        {
            // エラーが出るため一旦コメントアウト
            // Managerから現在の言語設定を取得して反映（後述のManager作成後に修正）
            // string currentLang = LocalizationManager.Instance.CurrentLanguage;
            // textComponent.text = table.GetText(textKey, currentLang);
        }
    }
}
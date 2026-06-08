using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// ローカライズされたテキスト表示を管理するクラス
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance;
        public string CurrentLanguage = "jp"; // 初期言語

        void Awake() { Instance = this; }

        public void ChangeLanguage(string langCode)
        {
            CurrentLanguage = langCode;
            // シーン内のすべてのLocalizedTextDisplayを更新する
            var displays = FindObjectsByType<LocalizedTextDisplay>(FindObjectsSortMode.None);
            foreach (var d in displays) d.UpdateText();
        }
    }
}
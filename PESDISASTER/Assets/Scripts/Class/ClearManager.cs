using UnityEngine;
using UnityEngine.UI;

namespace PESDISASTER
{
    /// <summary>
    /// クリアシーンの管理クラス
    /// </summary>
    public class ClearManager : MonoBehaviour
    {
        /// <summary>
        /// タイトルボタンの参照する変数
        /// </summary>
        public Button titleButton;

        /// <summary>
        /// タイトルシーン名を参照する変数
        /// </summary>
        private string titleSceneName = "Title";

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            titleButton.onClick.AddListener(MoveTitle);// タイトルボタンがクリックされたとき、GoScene関数を呼び出すように設定

            AudioManager.instance.PlayBGM(BGM_Type.Clear);
        }

        /// <summary>
        /// タイトルシーンの遷移を行う関数
        /// </summary>
        private void MoveTitle()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(titleSceneName);
        }
    }
}
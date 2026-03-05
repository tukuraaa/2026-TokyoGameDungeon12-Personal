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
        /// タイトルボタンの参照用変数
        /// </summary>
        private Button titleButton = null;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            titleButton = GameObject.Find("TitleButton").GetComponent<Button>();// シーン内からTitleButtonを探して取得
            titleButton.onClick.AddListener(MoveTitle);// タイトルボタンがクリックされたとき、GoScene関数を呼び出すように設定
        }

        /// <summary>
        /// タイトルシーンの遷移を行う関数
        /// </summary>
        void MoveTitle()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        }
    }
}
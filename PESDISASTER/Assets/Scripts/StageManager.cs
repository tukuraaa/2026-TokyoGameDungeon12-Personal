using UnityEngine;
using UnityEngine.UI;

namespace PESDISASTER
{
    /// <summary>
    /// ステージシーンの管理クラス
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        /// <summary>
        /// クリアボタンの参照用変数
        /// </summary>
        private Button clearButton = null;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            clearButton = GameObject.Find("ClearButton").GetComponent<Button>();// シーン内からClearButtonを探して取得
            clearButton.onClick.AddListener(MoveClear);// クリアボタンがクリックされたとき、GoScene関数を呼び出すように設定
        }

        /// <summary>
        /// ゲームクリアシーンへの遷移を行う関数
        /// </summary>
        private void MoveClear()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Clear");
        }
    }
}
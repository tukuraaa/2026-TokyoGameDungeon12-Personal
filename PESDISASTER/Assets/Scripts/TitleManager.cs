using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PESDISASTER
{
    /// <summary>
    /// タイトルシーンの管理クラス
    /// </summary>
    public class TitleManager : MonoBehaviour
    {
        /// <summary>
        /// スタートボタンの参照用変数
        /// </summary>
        private Button startButton = null;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            startButton = GameObject.Find("StartButton").GetComponent<Button>();// シーン内からStartButtonを探して取得
            startButton.onClick.AddListener(MoveStage);// スタートボタンがクリックされたとき、GoScene関数を呼び出すように設定
        }

        /// <summary>
        /// シーン遷移を行う関数
        /// </summary>
        public void MoveStage()
        {
            SceneManager.LoadScene("Stage");
        }
    }
}
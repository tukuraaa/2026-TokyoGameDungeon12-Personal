using System.Collections;
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
        /// アニメーターを参照する変数
        /// </summary>
        private Animator animator;

        /// <summary>
        /// 遷移演出用UIを管理するクラスを参照する変数
        /// </summary>
        public TransitionUI_Manager transitionUI_Manager;

        /// <summary>
        /// タイトルシーン名を参照する変数
        /// </summary>
        private string titleSceneName = "Title";

        /// <summary>
        /// クリアシーンアウトロアニメーションのパラメーターIDを参照する変数
        /// </summary>
        private static readonly int clearOutro_ID = Animator.StringToHash("OnTitle");

        /// <summary>
        /// アウトロ演出の持続時間を参照する変数
        /// </summary>
        private float outroEventDuration = 6f;
        /// <summary>
        /// イントロ演出の持続時間を参照する変数
        /// </summary>
        public float introEventDuration = 60f;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            animator = GetComponent<Animator>();
            titleButton.onClick.AddListener(MoveTitle);// タイトルボタンがクリックされたとき、GoScene関数を呼び出すように設定

            // カーソル設定
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;// カーソルを表示にする

            titleButton.enabled = false;// 最初はボタンの機能をオフ
            AudioManager.instance.PlayBGM(BGM_Type.Clear);
            StartCoroutine(Endroll_Coroutine());// エンドロール処理を開始
        }

        /// <summary>
        /// タイトルシーンの遷移を行う関数
        /// </summary>
        private void MoveTitle()
        {
            StartCoroutine(ClearOutroCoroutine());// クリアシーンのアウトロ処理を実行
        }

        /// <summary>
        /// エンドロールの処理を実行するコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator Endroll_Coroutine()
        {
            yield return new WaitForSeconds(introEventDuration);
            titleButton.enabled = true;// ボタンの機能をオン
        }

        /// <summary>
        /// クリアシーンのアウトロ処理を実行するコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator ClearOutroCoroutine()
        {
            titleButton.enabled = false;// 最初はボタンの機能をオフ
            AudioManager.instance.StopBGM();
            transitionUI_Manager.Show();
            animator.SetTrigger(clearOutro_ID);// アウトロ演出再生
            yield return new WaitForSeconds(outroEventDuration);
            UnityEngine.SceneManagement.SceneManager.LoadScene(titleSceneName);
        }
    }
}
using System.Collections;
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
        /// イントロ演出用のアニメーターを参照する変数
        /// </summary>
        private Animator introAnimator;

        /// <summary>
        /// 遷移演出用UIを管理するクラスを参照する変数
        /// </summary>
        public TransitionUI_Manager transitionUI_Manager;
        /// <summary>
        /// プレイヤーの操作UIを管理するクラスを参照する変数
        /// </summary>
        public PlayerControllerUI_Manager playerControllerUI_Manager;
        /// <summary>
        /// プレイヤー操作のUIを管理するクラスを参照する変数
        /// </summary>
        public PauseUI_Manager pauseUI_Manager;
        /// <summary>
        /// プレイヤー通知UIを管理するクラスを参照する変数
        /// </summary>
        public PlayerNoticeUI_Manager playerNoticeUI_Manager;

        /// <summary>
        /// 続行ボタンを参照する変数
        /// </summary>
        public Button resumeButton;
        /// <summary>
        /// タイトルボタンを参照する変数
        /// </summary>
        public Button titleButton;

        /// <summary>
        /// チュートリアル演出の持続時間を参照する変数
        /// </summary>
        private float tutorialDuration = 5.5f;
        /// <summary>
        /// 演出の持続時間を参照する変数
        /// </summary>
        public float introEventDuration = 16f;

        /// <summary>
        /// ポーズ解除後に時間を動かすための値を参照する変数
        /// </summary>
        private int timeCanMoveValue = 1;

        /// <summary>
        /// ポーズ中かどうかを示すフラグを参照する変数
        /// </summary>
        private bool isPausing = false;

        /// <summary>
        /// タイトルシーン名を参照する変数
        /// </summary>
        private string titleSceneName = "Title";

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            introAnimator = GetComponent<Animator>();

            // クリックイベントにリスナーを追加
            resumeButton.onClick.AddListener(Pause);// 続行ボタンがクリックされたとき、Pause関数を呼び出すように設定
            titleButton.onClick.AddListener(() => MoveScene(titleSceneName));// タイトルボタンがクリックされたとき、MoveScene関数を呼び出すように設定

            // カーソル設定
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;// カーソルを非表示にする

            transitionUI_Manager.Show();
            PlayerController.instance.isSleeping = true;
            StartCoroutine(IntroEventCoroutine());// イントロ演出を開始
        }

        /// <summary>
        /// 各シーンへの遷移を行う関数
        /// </summary>
        private void MoveScene(string name)
        {
            Time.timeScale = timeCanMoveValue;
            UnityEngine.SceneManagement.SceneManager.LoadScene(name);
        }

        /// <summary>
        /// イントロ演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator IntroEventCoroutine()
        {
            yield return new WaitForSeconds(introEventDuration);// 演出の持続時間を待つ
            transitionUI_Manager.Hide();
            playerControllerUI_Manager.StartTutorial();// 操作チュートリアルを開始する
            yield return new WaitForSeconds(tutorialDuration);// チュートリアル演出の持続時間を待つ
            playerNoticeUI_Manager.StartRule();// ゲーム目的演出を開始する
            OnIntroEnd();// イントロ演出の終了処理を呼び出す
        }

        /// <summary>
        /// イントロ演出の終了時にプレイヤーの操作を許可する関数
        /// </summary>
        private void OnIntroEnd()
        {
            PlayerController.instance.isSleeping = false;

            introAnimator.enabled = false;// イントロ用のアニメーターを止める
            
            PlayerController.instance.enabled = true;// プレイヤーの移動スクリプトを有効にする
        }

        /// <summary>
        /// ポーズの処理を行う関数
        /// </summary>
       public void Pause()
        {
            // もしポーズ中でない場合
            if (!isPausing)
            {
                Time.timeScale = 0f;
                pauseUI_Manager.Show();
                isPausing = true;

                // カーソル設定
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;// カーソルを表示にする
            }
            else
            {
                Time.timeScale = timeCanMoveValue;
                pauseUI_Manager.Hide();
                isPausing = false;

                // カーソル設定
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;// カーソルを非表示にする
            }
        }
    }
}
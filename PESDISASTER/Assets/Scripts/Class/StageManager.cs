using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private Animator animator;

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
        /// プレイヤーステータスUIを管理するクラスを参照する変数
        /// </summary>
        public PlayerStatusUI_Manager playerStatusUI_Manager;
        /// <summary>
        /// プレイヤーステータスUIを管理するクラスを参照する変数
        /// </summary>
        public GameOverUI_Manager gameOverUI_Manager;

        /// <summary>
        /// 続行ボタンを参照する変数
        /// </summary>
        public Button resumeButton;
        /// <summary>
        /// ポーズのタイトルボタンを参照する変数
        /// </summary>
        public Button pauseTitleButton;
        /// <summary>
        /// リトライボタンを参照する変数
        /// </summary>
        public Button retryButton;
        /// <summary>
        /// ゲームオーバーのタイトルボタンを参照する変数
        /// </summary>
        public Button overTitleButton;

        /// <summary>
        /// リトライボタンイベントを参照する変数
        /// </summary>
        public EventTrigger retryEvent;
        /// <summary>
        /// ゲームオーバーのタイトルボタンイベントを参照する変数
        /// </summary>
        public EventTrigger overTitleEvent;

        /// <summary>
        /// チュートリアル演出の時間を参照する変数
        /// </summary>
        private float tutorialDuration = 5.5f;
        /// <summary>
        /// ゲームオーバーアウトロ演出の時間を参照する変数
        /// </summary>
        private float overOutroDuration = 1.5f;
        /// <summary>
        /// ゲームオーバーイントロ演出の時間を参照する変数
        /// </summary>
        private float over_IntroDuration = 1f;
        /// <summary>
        /// 演出の持続時間を参照する変数
        /// </summary>
        public float introEventDuration = 16f;

        /// <summary>
        /// ポーズ解除後に時間を動かすための値を参照する変数
        /// </summary>
        private int timeCanMoveValue = 1;
        /// <summary>
        /// ゲームオーバー時イントロアニメーションのパラメーターIDを参照する変数
        /// </summary>
        private static readonly int over_Intro_ID = Animator.StringToHash("OnGameOver");
        /// <summary>
        /// ゲームオーバー時アウトロアニメーションのパラメーターIDを参照する変数
        /// </summary>
        private static readonly int overOutro_ID = Animator.StringToHash("OnOverOutro");

        /// <summary>
        /// ポーズ中かどうかを示すフラグを参照する変数
        /// </summary>
        private bool isPausing = false;

        /// <summary>
        /// タイトルシーン名を参照する変数
        /// </summary>
        private string titleSceneName = "Title";
        /// <summary>
        /// メインステージシーン名を参照する変数
        /// </summary>
        private string stageSceneName = "Stage";

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            animator = GetComponent<Animator>();

            // クリックイベントにリスナーを追加
            resumeButton.onClick.AddListener(Pause);// 続行ボタンがクリックされたとき、Pause関数を呼び出すように設定
            pauseTitleButton.onClick.AddListener(() => MoveScene(titleSceneName));// タイトルボタンがクリックされたとき、MoveScene関数を呼び出すように設定
            retryButton.onClick.AddListener(Retry);// リトライボタンがクリックされたとき、Retry関数を呼び出すように設定
            overTitleButton.onClick.AddListener(OverTitle);// ゲームオーバーのタイトルボタンがクリックされたとき、OverTitle関数を呼び出すように設定

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
        public void MoveScene(string name)
        {
            AudioManager.instance.StopBGM();
            Time.timeScale = timeCanMoveValue;
            UnityEngine.SceneManagement.SceneManager.LoadScene(name);
        }

        /// <summary>
        /// イントロ演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator IntroEventCoroutine()
        {
            AudioManager.instance.PlayBGM(BGM_Type.MainStage);
            yield return new WaitForSeconds(introEventDuration);// 演出の持続時間を待つ
            transitionUI_Manager.Hide();
            playerControllerUI_Manager.StartTutorial();// 操作チュートリアルを開始する
            yield return new WaitForSeconds(tutorialDuration);// チュートリアル演出の持続時間を待つ
            playerNoticeUI_Manager.StartRule();// ゲーム目的演出を開始する
            playerStatusUI_Manager.StartShow();// プレイヤーステータスUIを表示する
            OnIntroEnd();// イントロ演出の終了処理を呼び出す
        }

        /// <summary>
        /// イントロ演出の終了時にプレイヤーの操作を許可する関数
        /// </summary>
        private void OnIntroEnd()
        {
            PlayerController.instance.isSleeping = false;

            animator.enabled = false;// アニメーターを止める

            PlayerController.instance.enabled = true;// プレイヤーの移動スクリプトを有効にする
        }

        /// <summary>
        /// ポーズの処理を行う関数
        /// </summary>
        public void Pause()
        {
            AudioManager.instance.PlaySE(SE_Type.Pause);

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

        /// <summary>
        /// リトライ処理を呼び出す関数
        /// </summary>
        private void Retry()
        {
            StartCoroutine(GameOverOutroCoroutine(retryButton));// リトライ処理を実行
        }

        /// <summary>
        /// ゲームオーバーのアウトロ処理（リトライ処理・タイトル遷移処理）を実行するコルーチン
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        private IEnumerator GameOverOutroCoroutine(Button button)
        {
            // ボタン・ボタンイベントのアクセスを無効にする
            retryButton.enabled = false;// リトライボタンを最初は無効にする
            retryEvent.enabled = false;// リトライボタンイベントを最初は無効にする
            overTitleButton.enabled = false;// ゲームオーバーのタイトルボタンを最初は無効にする
            overTitleEvent.enabled = false;// ゲームオーバーのタイトルボタンイベントを最初は無効にする

           transitionUI_Manager.Show();
            animator.SetTrigger(overOutro_ID);// アウトロ再生
            yield return new WaitForSeconds(overOutroDuration);// 演出中は待機

            // もし押したボタンがリトライボタンの場合
            if (button == retryButton)
            {
                MoveScene(stageSceneName);// メインステージに遷移
            }
            // もし押したボタンがゲームオーバーのタイトルボタンの場合
            else if (button == overTitleButton)
            {
                MoveScene(titleSceneName);// タイトルに遷移
            }
        }

        /// <summary>
        /// ゲームオーバー時の処理を実行するコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator GameOverCoroutine()
        {
            animator.enabled = true;// アニメーターを起動
            PlayerController.instance.isSleeping = true;
            PlayerController.instance.enabled = false;// プレイヤーの移動スクリプトを無効にする
            gameOverUI_Manager.Show();

            // すでにかかっている曲を止めたうえでゲームオーバー用の曲を再生
            AudioManager.instance.StopBGM();
            AudioManager.instance.PlayBGM(BGM_Type.GameOver);

            // ボタン・ボタンイベントのアクセスを無効にする
            retryButton.enabled = false;// リトライボタンを最初は無効にする
            retryEvent.enabled = false;// リトライボタンイベントを最初は無効にする
            overTitleButton.enabled = false;// ゲームオーバーのタイトルボタンを最初は無効にする
            overTitleEvent.enabled = false;// ゲームオーバーのタイトルボタンイベントを最初は無効にする

            animator.SetTrigger(over_Intro_ID);// イントロ再生
            yield return new WaitForSeconds(over_IntroDuration);// 演出中は待機

            // ボタン・ボタンイベントのアクセスを有効にする
            retryButton.enabled = true;// リトライボタンを最初は有効にする
            retryEvent.enabled = true;// リトライボタンイベントを最初は有効にする
            overTitleButton.enabled = true;// ゲームオーバーのタイトルボタンを最初は有効にする
            overTitleEvent.enabled = true;// ゲームオーバーのタイトルボタンイベントを最初は有効にする

            // カーソル設定
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;// カーソルを表示にする
        }

        /// <summary>
        /// ゲームオーバー処理実行を呼び出す関数
        /// </summary>
        public void GameOver()
        {
            StartCoroutine(GameOverCoroutine());// ゲームオーバー処理を実行
        }

        /// <summary>
        /// ゲームオーバー時にタイトル遷移処理を呼び出す関数
        /// </summary>
        private void OverTitle()
        {
            StartCoroutine(GameOverOutroCoroutine(overTitleButton));// ゲームオーバー時のタイトル遷移処理を実行
        }
    }
}
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
        /// プレイヤーステータスUIを管理するクラスを参照する変数
        /// </summary>
        [SerializeField]
        private PlayerStatusUI_Manager _playerStatusUI_Manager;

        /// <summary>
        /// ゲームイントロ演出時間の倍率を参照する変数
        /// </summary>
        [SerializeField]
        private float _introTimeMultiplier = 1f;

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
        public GameOverUI_Manager gameOverUI_Manager;

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
        /// ステージマネージャーのインスタンスを参照する変数
        /// </summary>
        public static StageManager Instance { get; private set; }

        /// <summary>
        /// ゲームオーバーアウトロ演出の時間を参照する変数
        /// </summary>
        private float overOutroDuration = 1.5f;
        /// <summary>
        /// ゲームオーバーイントロ演出の時間を参照する変数
        /// </summary>
        private float over_IntroDuration = 1f;
        /// <summary>
        /// ゲームイントロ目覚める演出の持続時間を参照する変数
        /// </summary>
        private float _introWakeUpDuration = 11.5f;
        /// <summary>
        /// ゲームイントロ起きる演出の持続時間を参照する変数
        /// </summary>
        private float _introGetUpDuration = 3.5f;
        /// <summary>
        /// ゲームイントロ足着く演出の持続時間を参照する変数
        /// </summary>
        private float _introLandDuration = 2f;

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
        /// イントロ時の起きるアニメーションのパラメーターIDを参照する変数
        /// </summary>
        private static readonly int _introGetUp_ID = Animator.StringToHash("On_IntroGetUp");
        /// <summary>
        /// イントロ時の足着くアニメーションのパラメーターIDを参照する変数
        /// </summary>
        private static readonly int _introLand_ID = Animator.StringToHash("On_IntroLand");

        /// <summary>
        /// メインゲームスタートをしたかどうかを示すフラグを参照する変数
        /// </summary>
        public bool IsMainGameStarted = false;
        /// <summary>
        /// ポーズ中かどうかを示すフラグを参照する変数
        /// </summary>
        private bool isPausing = false;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // もしインスタンスが無い場合
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // コンポーネントの登録
            animator = GetComponent<Animator>();

            // --- クリックイベントにリスナーを追加 ---
            // リトライボタンがクリックされたとき、Retry関数を呼び出すように設定
            retryButton.onClick.AddListener(Retry);
            // ゲームオーバーのタイトルボタンがクリックされたとき、OverTitle関数を呼び出すように設定
            overTitleButton.onClick.AddListener(GoTitle);

            // --- カーソル設定 ---
            // カーソルをロックする
            Cursor.lockState = CursorLockMode.Locked;
            // カーソルを非表示にする
            Cursor.visible = false;
        }

        /// <summary>
        /// 起動時の初期処理を行う関数
        /// </summary>
        private void Start()
        {
            // --- イントロ演出準備 ---
            // プレイヤーの操作を禁止
            PlayerController.Instance.IsSleeping = true;

            // イントロ演出を開始
            StartCoroutine(IntroEventCoroutine());
        }

        /// <summary>
        /// 各シーンへの遷移を行う関数
        /// </summary>
        public void MoveScene(string name)
        {
            AudioManager.Instance.StopBGM();
            Time.timeScale = timeCanMoveValue;
            UnityEngine.SceneManagement.SceneManager.LoadScene(name);
        }

        /// <summary>
        /// イントロ演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator IntroEventCoroutine()
        {
            // --- イントロイベント初期処理 ---
            // 演出用UIを表示
            transitionUI_Manager.Show();
            // 廃屋BGSを再生
            AudioManager.Instance.PlaySE("HouseBGS");

            // --- 目覚める演出 ---
            // 目覚める演出の持続時間を待つ
            yield return new WaitForSeconds(_introWakeUpDuration * _introTimeMultiplier);
            // 遷移演出用UIを非表示
            transitionUI_Manager.Hide();

            // --- 起きる演出 ---
            // 起きるときのSEを再生
            AudioManager.Instance.PlaySE("GetUp");
            // 起きる演出開始
            animator.SetTrigger(_introGetUp_ID);
            // 起きる演出の持続時間を待つ
            yield return new WaitForSeconds(_introGetUpDuration * _introTimeMultiplier);
            // 起きるときのSEを停止
            AudioManager.Instance.StopLoopSE("GetUp");

            // --- 足着く演出 ---
            // 足着くときのSEを再生
            AudioManager.Instance.PlaySE("Land");
            // 足着く演出開始
            animator.SetTrigger(_introLand_ID);
            // 足着く演出の持続時間を待つ
            yield return new WaitForSeconds(_introLandDuration * _introTimeMultiplier);

            // --- 操作チュートリアル演出 ---
            // エイムUIを表示
            _playerStatusUI_Manager.StartAimUI_Show();
            // 操作チュートリアルを開始する
            playerControllerUI_Manager.StartPlayerDefaultTutorial();
            // イントロ演出の終了処理を呼び出す
            OnIntroEnd();
        }

        /// <summary>
        /// イントロ演出の終了時にプレイヤーの操作を許可する関数
        /// </summary>
        private void OnIntroEnd()
        {
            // プレイヤー操作停止フラグをオフ
            PlayerController.Instance.IsSleeping = false;
            // アニメーターを止める
            animator.enabled = false;
            // プレイヤーの操作スクリプトを有効にする
            PlayerController.Instance.enabled = true;
        }

        /// <summary>
        /// ポーズの処理を行う関数
        /// </summary>
        public void Pause()
        {
            AudioManager.Instance.PlaySE("Pause");

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
                // ゲームをやり直す
                RestartGame();
            }
            // もし押したボタンがゲームオーバーのタイトルボタンの場合
            else if (button == overTitleButton)
            {
                // タイトルに遷移
                MoveScene("Title");
            }
        }

        /// <summary>
        /// ゲームオーバー時の処理を実行するコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator GameOverCoroutine()
        {
            // アニメーターを起動
            animator.enabled = true;
            // プレイヤーの操作を禁止する
            PlayerController.Instance.IsSleeping = true;
            // プレイヤーの移動スクリプトを無効にする
            PlayerController.Instance.enabled = false;
            // ゲームオーバーUIを表示する
            gameOverUI_Manager.Show();

            // --- すでにかかっている曲を止めたうえでゲームオーバー用の曲を再生 ---
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlayBGM("GameOver");

            // --- ボタン・ボタンイベントのアクセスを無効にする ---
            // リトライボタンを最初は無効にする
            retryButton.enabled = false;
            // リトライボタンイベントを最初は無効にする
            retryEvent.enabled = false;
            // ゲームオーバーのタイトルボタンを最初は無効にする
            overTitleButton.enabled = false;
            // ゲームオーバーのタイトルボタンイベントを最初は無効にする
            overTitleEvent.enabled = false;

            // イントロ再生
            animator.SetTrigger(over_Intro_ID);
            // 演出中は待機
            yield return new WaitForSeconds(over_IntroDuration);

            // --- ボタン・ボタンイベントのアクセスを有効にする ---
            // リトライボタンを最初は有効にする
            retryButton.enabled = true;
            // リトライボタンイベントを最初は有効にする
            retryEvent.enabled = true;
            // ゲームオーバーのタイトルボタンを最初は有効にする
            overTitleButton.enabled = true;
            // ゲームオーバーのタイトルボタンイベントを最初は有効にする
            overTitleEvent.enabled = true;

            // --- カーソル設定 ---
            // カーソルのロックを解除する
            Cursor.lockState = CursorLockMode.None;
            // カーソルを表示にする
            Cursor.visible = true;
        }

        /// <summary>
        /// ゲームオーバー処理実行を呼び出す関数
        /// </summary>
        public void GameOver()
        {
            // ゲームオーバー処理を実行
            StartCoroutine(GameOverCoroutine());
        }

        /// <summary>
        /// ゲームオーバー時にタイトル遷移処理を呼び出す関数
        /// </summary>
        private void GoTitle()
        {
            // ゲームオーバー時のタイトル遷移処理を実行
            StartCoroutine(GameOverOutroCoroutine(overTitleButton));
        }

        /// <summary>
        /// ゲームをやり直す処理を行う関数
        /// </summary>
        public void RestartGame()
        {
            // メインステージに遷移
            MoveScene("Stage");
        }

        /// <summary>
        /// メインゲーム開始時の処理を行う関数
        /// </summary>
        public void MainGameStart()
        {
            // メインステージ（第一ステージ）のBGMを再生
            AudioManager.Instance.PlayBGM("Stage1");
            // プレイヤーのHPを表示
            PlayerStatusUI_Manager.Instance.StartHP_UI_Show();
            // ゲーム目的を記したUIを表示
            PlayerNoticeUI_Manager.Instance.NoticeRule();
        }
    }
}
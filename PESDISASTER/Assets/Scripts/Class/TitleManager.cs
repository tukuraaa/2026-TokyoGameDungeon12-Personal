using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace PESDISASTER
{
    /// <summary>
    /// タイトルシーンの管理クラス
    /// </summary>
    public class TitleManager : MonoBehaviour
    {
        /// <summary>
        /// スタートボタンを参照する変数
        /// </summary>
        public Button startButton;
        /// <summary>
        /// ゲーム終了ボタンを参照する変数
        /// </summary>
        public Button exitButton;

        /// <summary>
        /// 遷移演出用UIを管理するクラスを参照する変数
        /// </summary>
        public TransitionUI_Manager transitionUI_Manager;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator animator;

        /// <summary>
        /// アニメーターのタイトルアウトロトリガーを参照する変数
        /// </summary>
        private static readonly int titleOutroTrigger = Animator.StringToHash("OnStart");

        /// <summary>
        /// イントロアニメーションの時間を参照する変数
        /// </summary>
        private float introAnimDuration = 2f;
        /// <summary>
        /// アウトロアニメーションの時間を参照する変数
        /// </summary>
        private float outroAnimDuration = 1.5f;

        /// <summary>
        /// ステージシーン名を参照する変数
        /// </summary>
        private string stageName = "Stage";

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            animator = GetComponent<Animator>();

            // クリックイベントにリスナーを追加
            startButton.onClick.AddListener(MoveStage);// スタートボタンがクリックされたとき、GoScene関数を呼び出すように設定
            exitButton.onClick.AddListener(Exit);// ゲーム終了ボタンがクリックされたとき、Exit関数を呼び出すように設定

            // カーソル設定
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;// カーソルを非表示にする

            StartCoroutine(GameStartCoroutine());// ゲーム開始のコルーチンを開始
        }

        /// <summary>
        /// シーン遷移を行う関数
        /// </summary>
        public void MoveStage()
        {
            StartCoroutine(StageTransitionCoroutine());// ステージ遷移のコルーチンを開始
        }

        /// <summary>
        /// ゲーム開始時の演出を行うコルーチン関数
        /// </summary>
        /// <returns></returns>
        private IEnumerator GameStartCoroutine()
        {
            // ボタンのアクセスを無効にする
            startButton.enabled = false;// スタートボタンを最初は無効にする
            exitButton.enabled = false;// ゲーム終了ボタンを最初は無効にする

            transitionUI_Manager.Show();
            yield return new WaitForSeconds(introAnimDuration);// イントロアニメーションの時間だけ待機
            transitionUI_Manager.Hide();

            // ボタンのアクセスを有効にする
            startButton.enabled = true;// スタートボタンを有効にする
            exitButton.enabled = true;// ゲーム終了ボタンを有効にする
        }

        /// <summary>
        /// ステージ遷移時の演出を行うコルーチン関数
        /// </summary>
        /// <returns></returns>
        private IEnumerator StageTransitionCoroutine()
        {
            // ボタンのアクセスを無効にする
            startButton.enabled = false;// スタートボタンを最初は無効にする
            exitButton.enabled = false;// ゲーム終了ボタンを最初は無効にする
            
            transitionUI_Manager.Show();
            animator.SetTrigger(titleOutroTrigger);// タイトルアウトロトリガーを発動
            yield return new WaitForSeconds(outroAnimDuration);// アウトロアニメーションの時間だけ待機
            SceneManager.LoadScene(stageName);
        }

        /// <summary>
        /// ゲーム終了時の演出を行うコルーチン関数
        /// </summary>
        /// <returns></returns>
        private IEnumerator GameExitCoroutine()
        {
            // ボタンのアクセスを無効にする
            startButton.enabled = false;// スタートボタンを最初は無効にする
            exitButton.enabled = false;// ゲーム終了ボタンを最初は無効にする

            transitionUI_Manager.Show();
            animator.SetTrigger(titleOutroTrigger);// タイトルアウトロトリガーを発動
            yield return new WaitForSeconds(outroAnimDuration);// アウトロアニメーションの時間だけ待機
            Application.Quit();
            Debug.Log("ゲームを終了します。");
        }

        /// <summary>
        /// ゲーム終了の処理を行う関数
        /// </summary>
        private void Exit()
        {
            StartCoroutine(GameExitCoroutine());// ゲーム終了のコルーチンを開始
        }
    }
}
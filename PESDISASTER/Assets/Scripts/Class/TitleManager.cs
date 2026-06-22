using UnityEngine;
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
        /// 遷移演出用UIを管理するクラスを参照する変数
        /// </summary>
        [SerializeField]
        private TransitionUI_Manager _transitionUI_Manager;
        /// <summary>
        /// 遷移演出用UIを管理するクラスを参照する変数
        /// </summary>
        [SerializeField]
        private TitleUI_Manager _titleUI_Manager;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// アニメーターのタイトルアウトロトリガーを参照する変数
        /// </summary>
        private static readonly int _titleOutroTriggerID = Animator.StringToHash("OnStart");

        /// <summary>
        /// イントロアニメーションの時間を参照する変数
        /// </summary>
        private float _introAnimDuration = 2f;
        /// <summary>
        /// アウトロアニメーションの時間を参照する変数
        /// </summary>
        private float _outroAnimDuration = 1.5f;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // コンポーネント登録
            _animator = GetComponent<Animator>();

            // --- クリックイベントにリスナーを追加 ---
            // スタートボタンがクリックされたとき、MoveStage関数を呼び出すように設定
            _titleUI_Manager.StartButton.onClick.AddListener(MoveStage);
            // ゲーム終了ボタンがクリックされたとき、Exit関数を呼び出すように設定
            _titleUI_Manager.ExitButton.onClick.AddListener(Exit);
            // 言語変更ボタンがクリックされたとき、指定の言語変更関数を呼び出すように設定
            _titleUI_Manager.ChangeLanguageButton[0].onClick.AddListener(() => LocalizationManager.Instance.ChangeLanguage("English"));
            // 言語変更ボタンがクリックされたとき、指定の言語変更関数を呼び出すように設定
            _titleUI_Manager.ChangeLanguageButton[1].onClick.AddListener(() => LocalizationManager.Instance.ChangeLanguage("Chinese"));
            // 言語変更ボタンがクリックされたとき、指定の言語変更関数を呼び出すように設定
            _titleUI_Manager.ChangeLanguageButton[2].onClick.AddListener(() => LocalizationManager.Instance.ChangeLanguage("Japanese"));

            // ゲーム開始のコルーチンを開始
            StartCoroutine(GameStartCoroutine());
        }

        /// <summary>
        /// シーン遷移を行う関数
        /// </summary>
        public void MoveStage()
        {
            // ステージ遷移のコルーチンを開始
            StartCoroutine(StageTransitionCoroutine());
        }

        /// <summary>
        /// ゲーム開始時の演出を行うコルーチン関数
        /// </summary>
        /// <returns></returns>
        private IEnumerator GameStartCoroutine()
        {
            // 指定のBGMを再生
            AudioManager.instance.PlayBGM(BGM_Type.Title);
            // タイトルUIのボタン・イベントトリガーのアクセスをオフにする
            _titleUI_Manager.ChangeEnabled(false);
            // 演出用UIを表示
            _transitionUI_Manager.Show();
            // イントロアニメーションの時間だけ待機
            yield return new WaitForSeconds(_introAnimDuration);
            // 演出用UIを非表示
            _transitionUI_Manager.Hide();
            // タイトルUIのボタン・イベントトリガーのアクセスをオンにする
            _titleUI_Manager.ChangeEnabled(true);
        }

        /// <summary>
        /// ステージ遷移時の演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator StageTransitionCoroutine()
        {
            // --- クリック音再生とともにBGMを停止 ---
            AudioManager.instance.PlaySE(SE_Type.Click);
            AudioManager.instance.StopBGM();

            // --- メインステージへの遷移演出を行う ---
            // 最初はタイトルUIのボタン・イベントトリガーのアクセスをオフにする
            _titleUI_Manager.ChangeEnabled(false);
            // 演出用UIを表示
            _transitionUI_Manager.Show();
            // タイトルアウトロトリガーを発動
            _animator.SetTrigger(_titleOutroTriggerID);
            // アウトロアニメーションの時間だけ待機
            yield return new WaitForSeconds(_outroAnimDuration);
            // ステージシーンに遷移
            SceneManager.LoadScene("Stage");
        }

        /// <summary>
        /// ゲーム終了時の演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator GameExitCoroutine()
        {
            // --- クリック音再生とともにBGMを停止 ---
            AudioManager.instance.PlaySE(SE_Type.Click);
            AudioManager.instance.StopBGM();

            // --- ゲーム終了の演出を行う ---
            // タイトルUIのボタン・イベントトリガーのアクセスをオフにする
            _titleUI_Manager.ChangeEnabled(false);
            // 演出用UIを表示
            _transitionUI_Manager.Show();
            // タイトルアウトロトリガーを発動
            _animator.SetTrigger(_titleOutroTriggerID);
            // アウトロアニメーションの時間だけ待機
            yield return new WaitForSeconds(_outroAnimDuration);
            // アプリ終了
            Application.Quit();
        }

        /// <summary>
        /// ゲーム終了の処理を行う関数
        /// </summary>
        private void Exit()
        {
            // ゲーム終了のコルーチンを開始
            StartCoroutine(GameExitCoroutine());
        }
    }
}
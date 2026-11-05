using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PESDISASTER
{
    /// <summary>
    /// メモUIを管理するクラス
    /// </summary>
    public class PaperUIManager : MonoBehaviour
    {
        /// <summary>
        /// 戻るボタンを参照する変数
        /// </summary>
        [SerializeField]
        private Button _backButton;

        /// <summary>
        /// 指定のローカライズテキスト管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private LocalizedTextDisplay _targetLocalizedTextDisplay;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// このクラスのシングルトンインスタンスを参照する変数
        /// </summary>
        public static PaperUIManager Instance 
        {
            get; 
            private set; 
        }

        /// <summary>
        /// アニメーターのメモイベントイントロトリガーIDを参照する変数
        /// </summary>
        private static readonly int _introTriggerID = Animator.StringToHash("OnIntro");
        /// <summary>
        /// アニメーターのメモイベントアウトロトリガーIDを参照する変数
        /// </summary>
        private static readonly int _outroTriggerID = Animator.StringToHash("OnOutro");

        /// <summary>
        /// アニメーションの時間を参照する変数
        /// </summary>
        private float _animTime = 1.0f;

        /// <summary>
        /// 演出中か判別する変数
        /// </summary>
        private bool _isAnim = false;

        /// <summary>
        /// 紙SEの名前を参照する変数
        /// </summary>
        private string _paperSEName = "Paper";

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            if (Instance == null) { Instance = this; }
            else
            {
                this.enabled = false;
                Destroy(gameObject);
                return;
            }

            _animator = GetComponent<Animator>();

            _backButton.onClick.AddListener(HidePaper);

            // 最初は戻るボタンの機能をオフ
            _backButton.enabled = false;
            // 最初はUIを隠す
            UIGeneralManager.Instance.SetActiveChildUI(this.transform, false);
        }

        /// <summary>
        /// メモ内容を表示する処理の関数
        /// </summary>
        /// <param name="targetBarrier">指定のバリアを参照する変数</param>
        /// <param name="messageID">指定のメッセージIDを参照する変数</param>
        public void ShowPaper(BarrierManager targetBarrier, string messageID)
        {
            if (_isAnim) { return; }

            // メモの内容を表示するイントロ処理を呼び出し
            StartCoroutine(PaperAnimCoroutine(targetBarrier, messageID, _introTriggerID));
        }

        /// <summary>
        /// メモ内容を非表示する処理の関数
        /// </summary>
        private void HidePaper()
        {
            if (_isAnim) { return; }

            // メモの内容を非表示するアウトロ処理を呼び出し
            StartCoroutine(PaperAnimCoroutine(null, null, _outroTriggerID));
        }

        /// <summary>
        /// メモの内容を表示非表示するアニメーション処理を行うコルーチン
        /// </summary>
        /// <param name="targetBarrier">指定のバリアを参照する変数</param>
        /// <param name="messageID">指定のメッセージIDを参照する変数</param>
        /// <param name="animID">アニメーションのトリガーIDを参照する変数</param>
        /// <returns></returns>
        private IEnumerator PaperAnimCoroutine(BarrierManager targetBarrier, string messageID, int animID)
        {
            _isAnim = true;

            // もしイントロイベントの場合
            if (animID == _introTriggerID)
            {
                PlayerController.Instance.IsCanMove = false;
                UIGeneralManager.Instance.SetActiveChildUI(this.transform, true);

                // この紙オブジェクトに設定されているメッセージIDを現在のメッセージIDに変更
                _targetLocalizedTextDisplay.TextKey = messageID;
            }
            else
            {
                _backButton.enabled = false;

                // --- カーソルロック設定 ---------------
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                // --------------------------------------
            }

            AudioManager.Instance.PlaySE(_paperSEName);
            _animator.SetTrigger(animID);
            yield return new WaitForSeconds(_animTime);

            if (targetBarrier != null)
            {
                // バリアを削除を試みる
                targetBarrier.DestroyBarrier();
            }

            if (animID == _introTriggerID)
            {
                // --- カーソルロック解除 ---------------
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // --------------------------------------

                _backButton.enabled = true;
            }
            else
            {
                UIGeneralManager.Instance.SetActiveChildUI(this.transform, false);

                // もしメインゲームが開始されていない場合
                if (!StageManager.Instance.IsMainGameStarted)
                {
                    // メインゲームを開始するイベントを呼び出し
                    StageManager.Instance.MainGameStart();
                    yield return new WaitForSeconds(_animTime);
                }

                PlayerController.Instance.IsCanMove = true;
            }

            _isAnim = false;
        }
    }
}
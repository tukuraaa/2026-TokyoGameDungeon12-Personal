using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PESDISASTER
{
    /// <summary>
    /// メモのイベント管理するクラス
    /// </summary>
    public class PaperManager : MonoBehaviour, Paper_Interactable
    {
        /// <summary>
        /// メモUI管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private PaperUI_Manager _paperUI_Manager;

        /// <summary>
        /// バリア管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private BarrierManager _barrierManager;

        /// <summary>
        /// シングルトンインスタンスを参照する変数
        /// </summary>
        public static PaperManager Instance { get; private set; }

        /// <summary>
        /// 初めてメモを読んだか判別する変数
        /// </summary>
        public bool IsFirstRead = false;
        /// <summary>
        /// 演出中か判別する変数
        /// </summary>
        private bool _isAnim = false;

        /// <summary>
        /// アニメーションの時間を参照する変数
        /// </summary>
        private float _animTime = 1.0f;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // もしインスタンスが無い場合
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // --- ボタンイベントの登録 ---
            _paperUI_Manager.BackButton.onClick.AddListener(HidePaper);
        }

        /// <summary>
        /// プレイヤーがメモにアクセスした時に呼ばれる関数
        /// </summary>
        public void Interact()
        {
            // メモ内容を表示するイベント処理を呼び出し
            ShowPaper();
        }

        /// <summary>
        /// メモ内容を表示する処理の関数
        /// </summary>
        public void ShowPaper()
        {
            // もし演出中の場合
            if (_isAnim)
            {
                return;
            }

            // メモの内容を表示するイントロ処理を呼び出し
            StartCoroutine(PaperAnimCoroutine(PaperUI_Manager.Paper_Intro_ID,true));
        }

        /// <summary>
        /// メモ内容を非表示する処理の関数
        /// </summary>
        public void HidePaper()
        {
            // もし演出中の場合
            if (_isAnim)
            {
                return;
            }

            // メモの内容を非表示するアウトロ処理を呼び出し
            StartCoroutine(PaperAnimCoroutine(PaperUI_Manager.PaperOutro_ID, false));
        }

        /// <summary>
        /// メモの内容を表示非表示するアニメーション処理を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator PaperAnimCoroutine(int anim_ID, bool isActive)
        {
            // 演出中フラグをオン
            _isAnim = true;

            // もし表示処理の場合
            if (isActive)
            {
                // プレイヤー操作禁止
                PlayerController.Instance.IsSleeping = true;

                // --- 各プレイヤー操作入力値のリセット ---
                PlayerController.Instance.Move_Input = Vector2.zero;
                PlayerController.Instance.Look_Input = Vector2.zero;

                // オブジェクトを表示非表示
                _paperUI_Manager.ShowHide(isActive);
            }
            else
            {
                // --- カーソル設定 ---
                // カーソルをロックする
                Cursor.lockState = CursorLockMode.Locked;
                // カーソルを非表示にする
                Cursor.visible = false;
            }

            // 指定のSE再生
            AudioManager.Instance.PlaySE("Paper");
            // 指定のIDを再生
            _paperUI_Manager.PaperUI_Animator.SetTrigger(anim_ID);
            // 演出中待機
            yield return new WaitForSeconds(_animTime);

            // 参照しているバリア管理クラスがある場合
            if (_barrierManager != null)
            {
                // 初めて読んだかのフラグをオンにする
                IsFirstRead = true;
                // バリアを削除を試みる
                _barrierManager.DestroyBarrier();
            }

            // もし非表示処理の場合
            if (!isActive)
            {
                // オブジェクトを表示非表示
                _paperUI_Manager.ShowHide(isActive);

                // ボタン選択を解除
                EventSystem.current.SetSelectedGameObject(null);

                // もしメインゲームが開始されていない場合
                if (!StageManager.Instance.IsMainGameStarted)
                {
                    // メインゲームを開始するイベントを呼び出し
                    StageManager.Instance.MainGameStart();
                    // 演出中待機
                    yield return new WaitForSeconds(_animTime);
                    // メインゲーム開始フラグをオンにする
                    StageManager.Instance.IsMainGameStarted = true;
                }

                // プレイヤー操作許可
                PlayerController.Instance.IsSleeping = false;
            }
            else
            {
                // --- カーソル設定 ---
                // カーソルのロックを解除する
                Cursor.lockState = CursorLockMode.None;
                // カーソルを表示にする
                Cursor.visible = true;
            }

            // 演出中フラグをオフ
            _isAnim = false;
        }
    }
}
using UnityEngine;
using System.Collections;

namespace PESDISASTER
{
    /// <summary>
    /// プレイヤー通知UIを管理するクラス
    /// </summary>
    public class PlayerNoticeUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// 攻略ナビUIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _navigateNoticeUI_Target;
        /// <summary>
        /// ゲーム目的UIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _gameRuleUI_Target;
        /// <summary>
        /// マガジン空っぽUIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _magazineEmptyUI_Target;
        /// <summary>
        /// リロード完了UIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _reloadCompleteUI_Target;
        /// <summary>
        /// リロード失敗UIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _reloadFailedUI_Target;
        /// <summary>
        /// ロック中UIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _lockedUI_Target;
        /// <summary>
        /// 棚の開け方UIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _openShelfTutorial_UI_Target;
        /// <summary>
        /// 棚の開け方UIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _damageNoticeUI_Target;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// クラス自身のインスタンスを参照する変数
        /// </summary>
        public static PlayerNoticeUI_Manager Instance { get; private set; }

        /// <summary>
        /// アニメーターのゲーム目的トリガーを参照する変数
        /// </summary>
        private static readonly int _ruleTrigger_ID = Animator.StringToHash("OnRule");
        /// <summary>
        /// アニメーターのマガジン空っぽトリガーを参照する変数
        /// </summary>
        private static readonly int _emptyTrigger_ID = Animator.StringToHash("OnEmpty");
        /// <summary>
        /// アニメーターのリロード完了トリガーを参照する変数
        /// </summary>
        private static readonly int _reloadTrigger_ID = Animator.StringToHash("OnReload");
        /// <summary>
        /// アニメーターのリロード失敗トリガーを参照する変数
        /// </summary>
        private static readonly int _failedTrigger_ID = Animator.StringToHash("OnFailed");
        /// <summary>
        /// アニメーターのロック中トリガーを参照する変数
        /// </summary>
        private static readonly int _lockedTrigger_ID = Animator.StringToHash("OnLocked");
        /// <summary>
        /// アニメーターの棚の開け方トリガーを参照する変数
        /// </summary>
        private static readonly int _openTutorialTrigger_ID = Animator.StringToHash("OnOpenTutorial");
        /// <summary>
        /// アニメーターのダメージ時トリガーを参照する変数
        /// </summary>
        private static readonly int _damageTrigger_ID = Animator.StringToHash("OnDamage");
        /// <summary>
        /// アニメーターの攻略ナビ時トリガーを参照する変数
        /// </summary>
        private static readonly int _navigateTrigger_ID = Animator.StringToHash("OnNavigate");

        /// <summary>
        /// 通知アニメーションの時間を参照する変数
        /// </summary>
        private float _noticeAnimTime = 2f;
        /// <summary>
        /// 棚の開け方アニメーションの時間を参照する変数
        /// </summary>
        private float _openTutorialAnimTime = 3f;
        /// <summary>
        /// ダメージ時アニメーションの時間を参照する変数
        /// </summary>
        private float _damageAnimTime = 1f;

        /// <summary>
        /// アニメーション中かどうかを参照する変数
        /// </summary>
        private bool _isAnimating = false;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
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
            _animator = GetComponent<Animator>();

            // 最初はUIを非表示
            ShowHide(false);
        }

        /// <summary>
        /// 全てのUIを表示・非表示にする関数
        /// </summary>
        /// <param name="isActive"></param>
        private void ShowHide(bool isActive)
        {
            // すべての子オブジェクトを参照
            foreach (Transform child in transform)
            {
                // 子オブジェクトを非表示にする
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 指定のUIを表示・非表示する関数
        /// </summary>
        /// <param name="target"></param>
        /// <param name="isActive"></param>
        public void TargetShowHide(Transform target, bool isActive)
        {
            // 指定のUIを表示・非表示する
            target.gameObject.SetActive(isActive);
        }

        /// <summary>
        /// ゲーム目的表示を開始する関数
        /// </summary>
        public void NoticeRule()
        {
            // 指定のUIを表示する
            TargetShowHide(_gameRuleUI_Target, true);
            // アニメーターのゲーム目的トリガーを発動
            _animator.SetTrigger(_ruleTrigger_ID);
        }

        /// <summary>
        /// マガジン空っぽ表示を開始する関数
        /// </summary>
        public void NoticeEmpty()
        {
            // 通知のアニメーションを行う
            StartCoroutine(NoticeAnimCoroutine(_magazineEmptyUI_Target, _emptyTrigger_ID, _noticeAnimTime));
        }

        /// <summary>
        /// リロード完了表示を開始する関数
        /// </summary>
        public void NoticeReloadComplete()
        {
            // 通知のアニメーションを行う
            StartCoroutine(NoticeAnimCoroutine(_reloadCompleteUI_Target, _reloadTrigger_ID, _noticeAnimTime));
        }

        /// <summary>
        /// リロード失敗表示を開始する関数
        /// </summary>
        public void NoticeReloadFailed()
        {
            // 通知のアニメーションを行う
            StartCoroutine(NoticeAnimCoroutine(_reloadFailedUI_Target, _failedTrigger_ID, _noticeAnimTime));
        }

        /// <summary>
        /// 通知アニメーションを行うコルーチン
        /// </summary>
        /// <param name="target"></param>
        /// <param name="triggerNumber"></param>
        /// <returns></returns>
        private IEnumerator NoticeAnimCoroutine(Transform target, int triggerNumber, float time)
        {
            // もしアニメーション中の場合
            if (_isAnimating)
            {
                yield break;
            }

            // --- 通知アニメーション処理 ---
            // アニメーション中フラグをオンにする
            _isAnimating = true;
            // 指定のUIを表示する
            TargetShowHide(target, true);
            // 指定のトリガーを発動する
            _animator.SetTrigger(triggerNumber);
            // 指定の時間だけ待機する
            yield return new WaitForSeconds(time);
            // 指定のUIを非表示にする
            TargetShowHide(target, false);
            // アニメーション中フラグをオフにする
            _isAnimating = false;
        }

        /// <summary>
        /// ロック中表示を開始する関数
        /// </summary>
        public void NoticeLocked()
        {
            // 通知のアニメーションを行う
            StartCoroutine(NoticeAnimCoroutine(_lockedUI_Target, _lockedTrigger_ID, _noticeAnimTime));
        }

        /// <summary>
        /// 棚の開け方表示を開始する関数
        /// </summary>
        public void NoticeOpenShelfTutorial()
        {
            // 通知のアニメーションを行う
            StartCoroutine(NoticeAnimCoroutine(_openShelfTutorial_UI_Target, _openTutorialTrigger_ID, _openTutorialAnimTime));
        }

        /// <summary>
        /// ダメージ表示を開始する関数
        /// </summary>
        public void NoticeDamage()
        {
            // ダメージ通知のアニメーションを行う
            StartCoroutine(NoticeAnimCoroutine(_damageNoticeUI_Target, _damageTrigger_ID, _damageAnimTime));
        }

        /// <summary>
        /// 攻略ナビを開始する関数
        /// </summary>
        public void NoticeNavigate()
        {
            // 攻略ナビ通知のアニメーションを行う
            StartCoroutine(NoticeAnimCoroutine(_navigateNoticeUI_Target, _navigateTrigger_ID, _noticeAnimTime));
        }
    }
}
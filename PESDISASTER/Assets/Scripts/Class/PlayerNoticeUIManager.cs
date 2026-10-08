using UnityEngine;
using System.Collections;

namespace PESDISASTER
{
    /// <summary>
    /// プレイヤー通知UIを管理するクラス
    /// </summary>
    public class PlayerNoticeUIManager : MonoBehaviour
    {
        /// <summary>
        /// 攻略ナビUIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _navigateNoticeUITarget;
        /// <summary>
        /// ゲーム目的UIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _gameRuleUITarget;
        /// <summary>
        /// 被ダメージ通知UIのターゲットを参照する変数
        /// </summary>
        //[SerializeField]
        //private Transform _damageNoticeUI_Target;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// クラス自身のインスタンスを参照する変数
        /// </summary>
        public static PlayerNoticeUIManager Instance { get; private set; }

        /// <summary>
        /// アニメーターのゲーム目的トリガーを参照する変数
        /// </summary>
        private static readonly int _ruleTrigger_ID = Animator.StringToHash("OnRule");
        /// <summary>
        /// アニメーターのダメージ時トリガーを参照する変数
        /// </summary>
        //private static readonly int _damageTrigger_ID = Animator.StringToHash("OnDamage");
        /// <summary>
        /// アニメーターの攻略ナビ時トリガーを参照する変数
        /// </summary>
        private static readonly int _navigateTrigger_ID = Animator.StringToHash("OnNavigate");

        /// <summary>
        /// 通知アニメーションの時間を参照する変数
        /// </summary>
        private float _noticeAnimTime = 2f;
        /// <summary>
        /// ダメージ時アニメーションの時間を参照する変数
        /// </summary>
        //private float _damageAnimTime = 1f;

        /// <summary>
        /// アニメーション中かどうかを参照する変数
        /// </summary>
        private bool _isAnimating = false;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            _animator = GetComponent<Animator>();

            UIGeneralManager.Instance.SetActiveUI(false, this.transform);
        }

        /// <summary>
        /// ゲーム目的UIを表示する関数
        /// </summary>
        public void NoticeRule()
        {
            UIGeneralManager.Instance.SetActiveUI(true, _gameRuleUITarget);
            _animator.SetTrigger(_ruleTrigger_ID);
        }

        /// <summary>
        /// 指定アクションに反応する通知アニメーションを行うコルーチン
        /// </summary>
        /// <param name="target">指定のUIターゲットを参照する変数</param>
        /// <returns></returns>
        private IEnumerator NoticeActionCoroutine(Transform target)
        {
            if (_isAnimating)
            {
                yield break;
            }

            // --- 通知アニメーション処理 -----------------------------
            _isAnimating = true;
            UIGeneralManager.Instance.SetActiveUI(true, target);
            _animator.SetTrigger(_navigateTrigger_ID);
            yield return new WaitForSeconds(_noticeAnimTime);
            UIGeneralManager.Instance.SetActiveUI(false, target);
            _isAnimating = false;
            // --------------------------------------------------------
        }

        /// <summary>
        /// ダメージ表示を開始する関数
        /// </summary>
        /*public void NoticeDamage()
        {
            // ダメージ通知のアニメーションを行う
            StartCoroutine(NoticeActionCoroutine(_damageNoticeUI_Target, _damageTrigger_ID, _damageAnimTime));
        }*/

        /// <summary>
        /// 指定アクションに反応する通知を開始する関数
        /// </summary>
        public void NoticeAction()
        {
            // 指定アクションに反応する通知のアニメーションを行う
            StartCoroutine(NoticeActionCoroutine(_navigateNoticeUITarget));
        }
    }
}
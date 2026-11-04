using System.Collections;
using System.Xml.Linq;
using UnityEngine;

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
        /// インタラクト可能通知UIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _interactUITarget;

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
        /// クラス自身のインスタンスを参照する変数
        /// </summary>
        public static PlayerNoticeUIManager Instance { get; private set; }

        /// <summary>
        /// アニメーターのゲーム目的トリガーを参照する変数
        /// </summary>
        private static readonly int _ruleTrigger_ID = Animator.StringToHash("OnRule");
        /// <summary>
        /// アニメーターの攻略ナビ時トリガーを参照する変数
        /// </summary>
        private static readonly int _navigateTrigger_ID = Animator.StringToHash("OnNavigate");

        /// <summary>
        /// 通知アニメーションの時間を参照する変数
        /// </summary>
        private float _noticeAnimTime = 2f;

        /// <summary>
        /// アニメーション中かどうかを参照する変数
        /// </summary>
        private bool _isAnimating = false;

        /// <summary>
        /// 気になるものがあるときの文章ID名を参照する変数
        /// </summary>
        public string SomethingTextIDName = "ThereSomething";
        /// <summary>
        /// 現在のナビ通知文章ID名を参照する変数
        /// </summary>
        private string _currentNaviTextIDName;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else
            {
                this.enabled = false;
                Destroy(gameObject);
                return;
            }

            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// クラスの最初に呼び出される関数
        /// </summary>
        private void Start()
        {
            // 最初はUIの子要素を非表示
            UIGeneralManager.Instance.SetActiveChildUI(false, this.transform);
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
        /// インタラクト可能通知UIを表示する関数
        /// </summary>
        /// <param name="isActive">表示・非表示を判定するフラグを参照する変数</param>
        public void NoticeInteract(bool isActive)
        {
            UIGeneralManager.Instance.SetActiveUI(isActive, _interactUITarget);
        }

        /// <summary>
        /// 指定アクションに反応する通知アニメーションを行うコルーチン
        /// </summary>
        /// <param name="target">指定のUIターゲットを参照する変数</param>
        /// <param name="idName">文章のIDを参照する変数</param>
        /// <returns></returns>
        private IEnumerator NoticeActionCoroutine(Transform target, string idName)
        {
            if (_isAnimating)
            {
                yield break;
            }

            // --- 通知アニメーション処理 -----------------------------
            _isAnimating = true;

            UIGeneralManager.Instance.SetActiveUI(true, target);
            // 表示したいテキストに変更する
            ChangeTextMessage(idName);
            _animator.SetTrigger(_navigateTrigger_ID);
            yield return new WaitForSeconds(_noticeAnimTime);
            UIGeneralManager.Instance.SetActiveUI(false, target);
            
            _isAnimating = false;
            // --------------------------------------------------------
        }

        /// <summary>
        /// 指定アクションに反応する通知を開始する関数
        /// </summary>
        /// <param name="idName">文章ID名を参照する変数</param>
        public void NoticeAction(string idName)
        {
            // 指定アクションに反応する通知のアニメーションを行う
            StartCoroutine(NoticeActionCoroutine(_navigateNoticeUITarget, idName));
        }

        /// <summary>
        /// テキストの文章内容を変更する関数
        /// </summary>
        /// <param name="idName"></param>
        private void ChangeTextMessage(string idName)
        {
            // このチュートリアルが基本操作の場合
            if (idName == SomethingTextIDName)
            {
                // 現在のナビ通知文章ID名を気になるものがあるときの文章ID名に変更
                _currentNaviTextIDName = SomethingTextIDName;
            }

            // テキストを変更
            _targetLocalizedTextDisplay.TextKey = _currentNaviTextIDName;
        }
    }
}
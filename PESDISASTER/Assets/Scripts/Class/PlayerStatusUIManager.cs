using System.Collections;
using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// プレイヤーステータスUIを管理するクラス
    /// </summary>
    public class PlayerStatusUIManager : MonoBehaviour
    {
        /// <summary>
        /// ダメージエフェクトUIオブジェクトを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _damageEffectUI;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// エイムUIオブジェクトを参照する変数
        /// </summary>
        public Transform AimUI;
        /// <summary>
        /// 体力UIオブジェクトを参照する変数
        /// </summary>
        public Transform HP_UI;

        /// <summary>
        /// シングルトンインスタンスを参照する変数
        /// </summary>
        public static PlayerStatusUIManager Instance { get; private set; }

        /// <summary>
        /// アニメーターの体力UI表示トリガーを参照する変数
        /// </summary>
        private static readonly int _showHPTriggerID = Animator.StringToHash("OnHPShow");
        /// <summary>
        /// アニメーターのエイムUI表示トリガーを参照する変数
        /// </summary>
        private static readonly int _showAimTriggerID = Animator.StringToHash("OnAimShow");
        /// <summary>
        /// アニメーターのダメージ時トリガーを参照する変数
        /// </summary>
        private static readonly int _damageTriggerID = Animator.StringToHash("OnDamage");

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
        /// 指定UI表示を開始する関数
        /// </summary>
        public void ShowStatusUI(Transform target)
        {
            UIGeneralManager.Instance.SetActiveUI(true,target);

            if (target == HP_UI)
            {
                _animator.SetTrigger(_showHPTriggerID);
                return;
            }
            else if (target == AimUI)
            {
                _animator.SetTrigger(_showAimTriggerID);
                return;
            }
        }

        /// <summary>
        /// ダメージ表示を開始する関数
        /// </summary>
        public void NoticeDamage()
        {
            // ダメージエフェクトアニメーションを行う
            StartCoroutine(NoticeActionCoroutine());
        }

        /// <summary>
        /// ダメージエフェクトアニメーションを行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator NoticeActionCoroutine()
        {
            if (_isAnimating)
            {
                yield break;
            }

            // --- 通知アニメーション処理 -----------------------------
            _isAnimating = true;
            UIGeneralManager.Instance.SetActiveUI(true, _damageEffectUI);
            _animator.SetTrigger(_damageTriggerID);
            yield return new WaitForSeconds(_damageAnimTime);
            UIGeneralManager.Instance.SetActiveUI(false, _damageEffectUI);
            _isAnimating = false;
            // --------------------------------------------------------
        }
    }
}
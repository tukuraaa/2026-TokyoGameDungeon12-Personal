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
        //[SerializeField]
        //private Transform _damageEffectUI;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// エイムUIオブジェクトを参照する変数
        /// </summary>
        public Transform AimIcon;
        /// <summary>
        /// 体力UIオブジェクトを参照する変数
        /// </summary>
        public Transform HPIcon;

        /// <summary>
        /// UI表示を行うインターフェースを参照する変数
        /// </summary>
        private IUIViewer _uiViewerSource;

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
        //private static readonly int _damageTriggerID = Animator.StringToHash("OnDamage");

        /// <summary>
        /// ダメージ時アニメーションの時間を参照する変数
        /// </summary>
        //private float _damageAnimTime = 1f;

        /// <summary>
        /// アニメーション中かどうかを参照する変数
        /// </summary>
        //private bool _isAnimating = false;

        /// <summary>
        /// クラスの最初に呼び出される関数
        /// </summary>
        private void Start()
        {
            _animator = GetComponent<Animator>();

            // 最初はUIの子要素を非表示
            UIGeneralManager.Instance.SetActiveChildUI(this.transform, false);
        }

        /// <summary>
        /// イベント登録の初期設定を行う関数
        /// </summary>
        /// <param name="source">インターフェースのソースを参照する変数</param>
        public void Init(IUIViewer source)
        {
            _uiViewerSource = source;
            _uiViewerSource.OnSetActiveUI += SetActiveStatusUI;
        }

        /// <summary>
        /// 指定UI表示を開始する関数
        /// </summary>
        /// <param name="target">指定UIオブジェクト</param>
        /// <param name="isActive">表示状態</param>
        public void SetActiveStatusUI(Transform target, bool isActive)
        {
            UIGeneralManager.Instance.SetActiveUI(target, isActive);

            if (target == HPIcon)
            {
                _animator.SetTrigger(_showHPTriggerID);
                return;
            }
            else if (target == AimIcon)
            {
                _animator.SetTrigger(_showAimTriggerID);
                return;
            }
        }

        /*
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
            UIGeneralManager.Instance.SetActiveUI(_damageEffectUI, true);
            _animator.SetTrigger(_damageTriggerID);
            yield return new WaitForSeconds(_damageAnimTime);
            UIGeneralManager.Instance.SetActiveUI(_damageEffectUI, false);
            _isAnimating = false;
            // --------------------------------------------------------
        }
        */
    }
}
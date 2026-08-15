using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// プレイヤーステータスUIを管理するクラス
    /// </summary>
    public class PlayerStatusUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// エイムUIオブジェクトを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _aimUI;
        /// <summary>
        /// 体力UIオブジェクトを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _hP_UI;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// シングルトンインスタンスを参照する変数
        /// </summary>
        public static PlayerStatusUI_Manager Instance { get; private set; }

        /// <summary>
        /// アニメーターの体力UI表示トリガーを参照する変数
        /// </summary>
        private static readonly int _showHP_Trigger = Animator.StringToHash("OnHP_Show");
        /// <summary>
        /// アニメーターのエイムUI表示トリガーを参照する変数
        /// </summary>
        private static readonly int _showAimTrigger = Animator.StringToHash("OnAimShow");

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // もしインスタンスが存在しない場合
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // --- コンポーネントの登録 ---
            _animator = GetComponent<Animator>();

            // 最初はUIを非表示
            Hide();
        }

        /// <summary>
        /// UIを非表示にする関数
        /// </summary>
        public void Hide()
        {
            // 子オブジェクトを全てチェック
            foreach (Transform child in transform)
            {
                // 子オブジェクトを非表示
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// UIを指定して表示する関数
        /// </summary>
        public void TargetShow(Transform target)
        {
            // 指定のオブジェクトを表示
            target.gameObject.SetActive(true);
        }

        /// <summary>
        /// エイムUI表示を開始する関数
        /// </summary>
        public void StartAimUI_Show()
        {
            // 指定のオブジェクトを表示
            TargetShow(_aimUI);
            // アニメーターのエイムUI表示トリガーを発動
            _animator.SetTrigger(_showAimTrigger);
        }

        /// <summary>
        /// 体力UI表示を開始する関数
        /// </summary>
        public void StartHP_UI_Show()
        {
            // 指定のオブジェクトを表示
            TargetShow(_hP_UI);
            // アニメーターの体力UI表示トリガーを発動
            _animator.SetTrigger(_showHP_Trigger);
        }
    }
}
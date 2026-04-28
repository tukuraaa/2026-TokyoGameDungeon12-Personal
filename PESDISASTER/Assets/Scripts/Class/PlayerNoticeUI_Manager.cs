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
        /// アニメーターを参照する変数
        /// </summary>
        private Animator animator;

        /// <summary>
        /// ゲーム目的UIのターゲットを参照する変数
        /// </summary>
        public Transform gameRuleUI_Target;

        /// <summary>
        /// アニメーターのゲーム目的トリガーを参照する変数
        /// </summary>
        public static readonly int ruleTrigger = Animator.StringToHash("OnRule");

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            animator = GetComponent<Animator>();
            Hide();
        }

        /// <summary>
        /// UIを非表示にする関数
        /// </summary>
        private void Hide()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// UIを指定して表示する関数
        /// </summary>
        public void TargetShow(Transform target)
        {
            target.gameObject.SetActive(true);
        }

        /// <summary>
        /// UIを指定して非表示にする関数
        /// </summary>
        /// <param name="target"></param>
        public void TargetHide(Transform target)
        {
            {
                target.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// ゲーム目的表示を開始する関数
        /// </summary>
        public void StartRule()
        {
            TargetShow(gameRuleUI_Target);
            animator.SetTrigger(ruleTrigger);// アニメーターのゲーム目的トリガーを発動
        }
    }
}
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
        /// マガジン空っぽUIのターゲットを参照する変数
        /// </summary>
        public Transform magazineEmptyUI_Target;
        /// <summary>
        /// リロード完了UIのターゲットを参照する変数
        /// </summary>
        public Transform reloadCompleteUI_Target;
        /// <summary>
        /// リロード失敗UIのターゲットを参照する変数
        /// </summary>
        public Transform reloadFailedUI_Target;
        /// <summary>
        /// ロック中UIのターゲットを参照する変数
        /// </summary>
        public Transform lockedUI_Target;
        /// <summary>
        /// 棚の開け方UIのターゲットを参照する変数
        /// </summary>
        public Transform openShelfTutorial_UI_Target;

        /// <summary>
        /// アニメーターのゲーム目的トリガーを参照する変数
        /// </summary>
        public static readonly int ruleTrigger = Animator.StringToHash("OnRule");
        /// <summary>
        /// アニメーターのマガジン空っぽトリガーを参照する変数
        /// </summary>
        public static readonly int emptyTrigger = Animator.StringToHash("OnEmpty");
        /// <summary>
        /// アニメーターのリロード完了トリガーを参照する変数
        /// </summary>
        public static readonly int reloadTrigger = Animator.StringToHash("OnReload");
        /// <summary>
        /// アニメーターのリロード失敗トリガーを参照する変数
        /// </summary>
        public static readonly int failedTrigger = Animator.StringToHash("OnFailed");
        /// <summary>
        /// アニメーターのロック中トリガーを参照する変数
        /// </summary>
        public static readonly int lockedTrigger = Animator.StringToHash("OnLocked");
        /// <summary>
        /// アニメーターの棚の開け方トリガーを参照する変数
        /// </summary>
        public static readonly int openTutorialTrigger = Animator.StringToHash("OnOpenTutorial");

        /// <summary>
        /// 通知アニメーションの時間を参照する変数
        /// </summary>
        private float noticeAnimTime = 2f;
        /// <summary>
        /// 棚の開け方アニメーションの時間を参照する変数
        /// </summary>
        private float openTutorialAnimTime = 3f;

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

        /// <summary>
        /// マガジン空っぽ表示を開始する関数
        /// </summary>
        public void StartEmpty()
        {
            StartCoroutine(NoticeAnimCoroutine(magazineEmptyUI_Target, emptyTrigger,noticeAnimTime));// 通知のアニメーションを行う
        }

        /// <summary>
        /// リロード完了表示を開始する関数
        /// </summary>
        public void StartReload()
        {
            StartCoroutine(NoticeAnimCoroutine(reloadCompleteUI_Target, reloadTrigger,noticeAnimTime));// 通知のアニメーションを行う
        }

        /// <summary>
        /// リロード失敗表示を開始する関数
        /// </summary>
        public void StartFailed()
        {
            StartCoroutine(NoticeAnimCoroutine(reloadFailedUI_Target, failedTrigger,noticeAnimTime));// 通知のアニメーションを行う
        }

        /// <summary>
        /// 通知アニメーションを行うコルーチン
        /// </summary>
        /// <param name="target"></param>
        /// <param name="triggerNumber"></param>
        /// <returns></returns>
        private IEnumerator NoticeAnimCoroutine(Transform target, int triggerNumber, float time)
        {
            TargetShow(target);
            animator.SetTrigger(triggerNumber);
            yield return new WaitForSeconds(time);
            TargetHide(target);
        }

        /// <summary>
        /// ロック中表示を開始する関数
        /// </summary>
        public void StartLocked()
        {
            StartCoroutine(NoticeAnimCoroutine(lockedUI_Target, lockedTrigger,noticeAnimTime));// 通知のアニメーションを行う
        }

        /// <summary>
        /// 棚の開け方表示を開始する関数
        /// </summary>
        public void StartOpenTutorial()
        {
            StartCoroutine(NoticeAnimCoroutine(openShelfTutorial_UI_Target, openTutorialTrigger, openTutorialAnimTime));// 通知のアニメーションを行う
        }
    }
}
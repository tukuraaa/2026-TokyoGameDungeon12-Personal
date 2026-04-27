using System.Collections;
using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// プレイヤー操作UIを管理するクラス
    /// </summary>
    public class PlayerControllerUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator animator;

        /// <summary>
        /// 移動コントロールUIのターゲットを参照する変数
        /// </summary>
        public Transform moveControl_UI_Target;
        /// <summary>
        /// 視点コントロールUIのターゲットを参照する変数
        /// </summary>
        public Transform lookControl_UI_Target;
        /// <summary>
        /// ポーズコントロールUIのターゲットを参照する変数
        /// </summary>
        public Transform pauseControl_UI_Target;

        /// <summary>
        /// アニメーターの操作チュートリアルトリガーを参照する変数
        /// </summary>
        public static readonly int control_TutorialTrigger1 = Animator.StringToHash("OnTutorial1");

        /// <summary>
        /// チュートリアル演出時間を参照する変数
        /// </summary>
        private float tutorialDuration = 4.5f;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        void Start()
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
        /// チュートリアル演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator TutorialCoroutine()
        {
            TargetShow(moveControl_UI_Target);
            TargetShow(lookControl_UI_Target);
            TargetShow(pauseControl_UI_Target);
            animator.SetTrigger(control_TutorialTrigger1);// アニメーターの操作チュートリアルトリガーを発動
            yield return new WaitForSeconds(tutorialDuration);
            TargetHide(moveControl_UI_Target);
            TargetHide(lookControl_UI_Target);
        }

        /// <summary>
        /// 操作チュートリアルを開始する関数
        /// </summary>
        public void StartTutorial()
        {
            StartCoroutine(TutorialCoroutine());
        }
    }
}
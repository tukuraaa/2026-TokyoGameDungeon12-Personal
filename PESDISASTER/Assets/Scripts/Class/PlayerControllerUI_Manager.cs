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
        /// 移動コントロールUIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _moveControl_UI_Target;
        /// <summary>
        /// 視点コントロールUIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _lookControl_UI_Target;
        /// <summary>
        /// ポーズコントロールUIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _pauseControl_UI_Target;
        /// <summary>
        /// 銃の操作UIのターゲットを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _handgunControl_UI_Target;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// アニメーターの操作チュートリアルトリガー1を参照する変数
        /// </summary>
        private static readonly int _controlTutorialTriggerLookMove = Animator.StringToHash("OnTutorial_LookMove");
        /// <summary>
        /// アニメーターの操作チュートリアルトリガー2を参照する変数
        /// </summary>
        private static readonly int _controlTutorialTrigger2 = Animator.StringToHash("OnTutorial2");

        /// <summary>
        /// チュートリアル演出時間を参照する変数
        /// </summary>
        private float _tutorialDuration = 9f;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            _animator = GetComponent<Animator>();
            Hide();
        }

        /// <summary>
        /// UIを非表示にする関数
        /// </summary>
        private void Hide()
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
            // 指定オブジェクトを表示
            target.gameObject.SetActive(true);
        }

        /// <summary>
        /// UIを指定して非表示にする関数
        /// </summary>
        /// <param name="target"></param>
        public void TargetHide(Transform target)
        {
            // 指定オブジェクトを表示
            target.gameObject.SetActive(false);
        }

        /// <summary>
        /// 視点・動作の移動方法チュートリアル演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator TutorialCoroutineLookMove()
        {
            // --- 指定のUIを表示 ---
            TargetShow(_moveControl_UI_Target);
            TargetShow(_lookControl_UI_Target);

            // アニメーターの操作チュートリアルトリガーを発動
            _animator.SetTrigger(_controlTutorialTriggerLookMove);
            // 演出時間分待機
            yield return new WaitForSeconds(_tutorialDuration);

            // --- 指定のUIを非表示 ---
            TargetHide(_moveControl_UI_Target);
            TargetHide(_lookControl_UI_Target);
        }

        /// <summary>
        /// 操作チュートリアルを開始する関数
        /// </summary>
        public void StartTutorial_LookMove()
        {
            // チュートリアル演出を呼び出し
            StartCoroutine(TutorialCoroutineLookMove());
        }

        /// <summary>
        /// 銃操作チュートリアルを開始する関数
        /// </summary>
        public void StartGunTutorial()
        {
            // 指定のUIを表示
            TargetShow(_handgunControl_UI_Target);
            // アニメーターの操作チュートリアルトリガー2を発動
            _animator.SetTrigger(_controlTutorialTrigger2);
        }
    }
}
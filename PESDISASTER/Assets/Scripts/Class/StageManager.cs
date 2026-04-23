using System.Collections;
using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// ステージシーンの管理クラス
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        /// <summary>
        /// イントロ演出用のアニメーターを参照する変数
        /// </summary>
        private Animator introAnimator;

        /// <summary>
        /// 遷移演出用UIを管理するクラスを参照する変数
        /// </summary>
        public TransitionUI_Manager transitionUI_Manager;
        /// <summary>
        /// プレイヤーの操作UIを管理するクラスを参照する変数
        /// </summary>
        public PlayerControllerUI_Manager playerControllerUI_Manager;

        /// <summary>
        /// 演出の持続時間を参照する変数
        /// </summary>
        public float introEventDuration = 16f;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            introAnimator = GetComponent<Animator>();
            transitionUI_Manager.Show();
            PlayerController.instance.isSleeping = true;
            StartCoroutine(IntroEventCoroutine());// イントロ演出を開始
        }

        /// <summary>
        /// ゲームクリアシーンへの遷移を行う関数
        /// </summary>
        private void MoveClear()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Clear");
        }

        /// <summary>
        /// イントロ演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator IntroEventCoroutine()
        {
            yield return new WaitForSeconds(introEventDuration);// 演出の持続時間を待つ
            transitionUI_Manager.Hide();
            PlayerController.instance.isSleeping = false;
            OnIntroEnd();// イントロ演出の終了処理を呼び出す
        }

        /// <summary>
        /// イントロ演出の終了時にプレイヤーの操作を許可する関数
        /// </summary>
        private void OnIntroEnd()
        {
            introAnimator.enabled = false;// イントロ用のアニメーターを止める
         
            PlayerController.instance.enabled = true;// プレイヤーの移動スクリプトを有効にする

            playerControllerUI_Manager.StartTutorial();// 操作チュートリアルを開始する
        }
    }
}
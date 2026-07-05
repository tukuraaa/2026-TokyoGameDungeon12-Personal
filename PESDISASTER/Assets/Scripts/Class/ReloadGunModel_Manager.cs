using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// リロードイベント用銃モデルを管理するクラス
    /// </summary>
    public class ReloadGunModel_Manager : MonoBehaviour
    {
        /// <summary>
        /// ハンドガンオブジェクトを参照する変数
        /// </summary>
        public Transform Handgun;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        public Animator ReloadModel_Amimator;

        /// <summary>
        /// ハンドガンのステージ1トリガーIDを参照する変数
        /// </summary>
        public static readonly int HandgunStage1_TriggerID = Animator.StringToHash("OnHandgunStage1");
        /// <summary>
        /// リロードミニゲーム終了トリガーIDを参照する変数
        /// </summary>
        public static readonly int MinigameEndTriggerID = Animator.StringToHash("OnMinigameEnd");
        /// <summary>
        /// ハンドガンのステージ2トリガーIDを参照する変数
        /// </summary>
        public static readonly int HandgunStage2_TriggerID = Animator.StringToHash("OnHandgunStage2");

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
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
            // 指定オブジェクトを表示
            target.gameObject.SetActive(true);
        }
    }
}

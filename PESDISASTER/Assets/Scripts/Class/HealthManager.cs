using UnityEngine;
using UnityEngine.UI;

namespace PESDISASTER
{
    /// <summary>
    /// 指定オブジェクトの体力を管理するクラス
    /// </summary>
    public class HealthManager : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーのHPバーを参照する変数
        /// </summary>
        public Image playerHP_Bar;

        /// <summary>
        /// 敵の行動を管理するクラスを参照する変数
        /// </summary>
        public EnemyAI enemyAI;

        /// <summary>
        /// 体力を参照する変数
        /// </summary>
        private float currentHealth;
        /// <summary>
        /// ダメージ時のプレイヤーHPバーの減り具合を参照する変数
        /// </summary>
        private float decreaseAmount = 0.30f;
        /// <summary>
        /// 体力の最大値を参照する変数
        /// </summary>
        public float maxHealth = 100f;

        /// <summary>
        /// プレイヤーのタグを参照する変数
        /// </summary>
        private string playerTag = "Player";

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            currentHealth = maxHealth;// 体力を最大値で初期化
        }

        /// <summary>
        /// ダメージを受けるための関数
        /// </summary>
        /// <param name="amount"></param>
        public void TakeDamage(float amount)
        {
            currentHealth -= amount;// ダメージを体力から減算

            // もしこのクラスがアタッチされているオブジェクトにプレイヤータグがついている場合
            if (this.CompareTag(playerTag))
            {
                // もし体力が0より多い場合
                if (currentHealth > 0)
                {
                    playerHP_Bar.fillAmount -= decreaseAmount;
                }
            }
            else
            {
                // もし体力が0より多い場合
                if (currentHealth > 0)
                {
                    enemyAI.Damage();// 敵の被ダメージの演出を実行
                }
            }

            // もし体力が0以下になった場合
            if (currentHealth <= 0)
            {
                // もしこのクラスがアタッチされているオブジェクトにプレイヤータグがついている場合
                if (this.CompareTag(playerTag))
                {
                    // すでにかかっている曲を止めたうえでゲームオーバー用の曲を再生
                    AudioManager.instance.StopBGM();
                    AudioManager.instance.PlayBGM(BGM_Type.GameOver);
                }
                else
                {
                    enemyAI.Die();// 敵の撃退演出を行う
                    this.enabled = false;// 機能を停止
                }
            }
        }
    }
}
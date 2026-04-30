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
                playerHP_Bar.fillAmount -= decreaseAmount;
            }

            // もし体力が0以下になった場合
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// オブジェクトが死亡するための関数
        /// </summary>
        private void Die()
        {
            Destroy(gameObject);
        }
    }
}
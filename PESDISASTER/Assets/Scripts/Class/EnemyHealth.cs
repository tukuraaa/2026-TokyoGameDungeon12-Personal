using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// 敵の体力を管理するクラス
    /// </summary>
    public class EnemyHealth : MonoBehaviour
    {
        /// <summary>
        /// 体力を参照する変数
        /// </summary>
        private float currentHealth;
        /// <summary>
        /// 体力の最大値を参照する変数
        /// </summary>
        public float maxHealth = 100f;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        void Start()
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
            Debug.Log(gameObject.name + " がダメージを受けた！ 残りHP: " + currentHealth);

            // もし体力が0以下になった場合
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// 敵が死亡するための関数
        /// </summary>
        private void Die()
        {
            Destroy(gameObject);
        }
    }
}
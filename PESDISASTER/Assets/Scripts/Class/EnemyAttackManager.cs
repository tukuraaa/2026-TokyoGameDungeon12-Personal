using UnityEngine;
using UnityEngine.Events;

namespace PESDISASTER
{
    public class EnemyAttackManager: MonoBehaviour
    {
        /// <summary>
        /// 攻撃のあたり判定を参照する変数
        /// </summary>
        private Collider attackCollider;

        /// <summary>
        /// プレイヤー通知UIを管理するクラスを参照する変数
        /// </summary>
        public PlayerNoticeUI_Manager playerNoticeUI_Manager;

        /// <summary>
        /// プレイヤーのタグを参照する変数
        /// </summary>
        private string playerTag = "Player";

        /// <summary>
        /// 対象に与えるダメージ量を参照する変数
        /// </summary>
        private float damage = 30f;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            attackCollider = GetComponent<Collider>();
            attackCollider.enabled = false;// 最初は攻撃の当たり判定を消しておく
        }

        /// <summary>
        /// トリガーコライダーに何かが侵入したときに呼ばれる関数
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            // 接触した相手（other）のタグが "Playe" かどうかを判定
            if (other.CompareTag(playerTag))
            {
                attackCollider.enabled = false;// 多段ヒットを避けるためにコライダーを無力化

                HealthManager player = other.GetComponent<HealthManager>();// 当たった相手にHealthManagerスクリプトがついているか確認

                // もしHealthManagerスクリプトがついている場合
                if (player != null)
                {
                    playerNoticeUI_Manager.StartDamageNotice();// ダメージ通知のアニメーションを実行
                    player.TakeDamage(damage);
                }
            }
        }
    }
}
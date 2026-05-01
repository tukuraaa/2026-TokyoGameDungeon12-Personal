using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace PESDISASTER
{
    /// <summary>
    /// 敵のAIを制御するクラス
    /// </summary>
    public class EnemyAI : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーの位置を参照する変数
        /// </summary>
        public Transform playerTransform;

        /// <summary>
        /// 道路網エージェントを参照する変数
        /// </summary>
        private NavMeshAgent agent;

        /// <summary>
        /// アニメーターコンポーネントを参照する変数
        /// </summary>
        public Animator animator;

        /// <summary>
        /// 攻撃のあたり判定を参照する変数
        /// </summary>
        public Collider attackCollider;

        /// <summary>
        /// 壁などのレイヤーを参照する変数
        /// </summary>
        public LayerMask obstacleMask;

        /// <summary>
        /// 歩きアニメーションのパラメーター値を参照する変数
        /// </summary>
        private float canWalkValue = 1f;
        /// <summary>
        /// 攻撃の連打を防ぐためのタイマーを参照する変数
        /// </summary>
        private float attackCooldown = 2f;
        /// <summary>
        /// 攻撃判定出現前演出タイマーを参照する変数
        /// </summary>
        private float attackBeforeTime = 1f;
        /// <summary>
        /// 攻撃判定出現タイマーを参照する変数
        /// </summary>
        private float attackTime = 2f;
        /// <summary>
        /// ダメージアニメーションタイマーを参照する変数
        /// </summary>
        private float damageTime = 1.2f;
        /// <summary>
        /// 撃退アニメーションタイマーを参照する変数
        /// </summary>
        private float defeatTime = 4f;
        /// <summary>
        /// 気づいて追いかけ始める距離を参照する変数
        /// </summary>
        public float chaseRange = 15f;
        /// <summary>
        /// 攻撃が届く距離を参照する変数
        /// </summary>
        public float attackRange = 2f;
        /// <summary>
        /// 視野角の片角度を参照する変数
        /// </summary>
        public float detectionAngle = 60f;

        /// <summary>
        /// プレイヤーのタグを参照する変数
        /// </summary>
        private string playerTag = "Player";

        /// <summary>
        /// 歩くアニメーションのパラメーターIDを参照する変数
        /// </summary>
        private static readonly int walk_ID = Animator.StringToHash("Walk");
        /// <summary>
        /// 攻撃アニメーションのパラメーターIDを参照する変数
        /// </summary>
        private static readonly int attack_ID = Animator.StringToHash("Attack");
        /// <summary>
        /// 敗北アニメーションのパラメーターIDを参照する変数
        /// </summary>
        private static readonly int defeat_ID = Animator.StringToHash("Defeat");
        /// <summary>
        /// ダメージアニメーションのパラメーターIDを参照する変数
        /// </summary>
        private static readonly int damage_ID = Animator.StringToHash("Damage");

        /// <summary>
        /// 攻撃中かどうかを管理するフラグを参照する変数
        /// </summary>
        private bool isAttacking = false;
        /// <summary>
        /// 撃退演出中かどうかを管理するフラグを参照する変数
        /// </summary>
        private bool isDefeating = false;
        /// <summary>
        /// 被ダメージ演出中かどうかを管理するフラグを参照する変数
        /// </summary>
        private bool isDamaging = false;

        /// <summary>
        /// 敵の状態定義の列挙型
        /// </summary>
        public enum EnemyState
        {
            Idle,
            Chasing,
            Attacking
        }
        EnemyState currentState = EnemyState.Idle;// 初期状態は待機

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();

            // もしプレイヤーがセットされていない場合
            if (playerTransform == null)
            {
                GameObject playerObj = GameObject.FindWithTag(playerTag);

                // プレイヤーオブジェクトが見つかった場合
                if (playerObj != null)
                {
                    playerTransform = playerObj.transform;// そのTransformを参照する
                }
            }
        }

        /// <summary>
        /// 毎フレーム処理を行う関数
        /// </summary>
        private void Update()
        {
            // もしプレイヤーが見つからない場合
            if (playerTransform == null || isAttacking || isDefeating || isDamaging)
            {
                return;
            }

            DetermineState();// 状態の決定

            ExecuteAction();// 状態に応じた行動の実行
        }

        /// <summary>
        /// プレイヤーとの距離を測り、今の状態を決定する関数
        /// </summary>
        private void DetermineState()
        {
            // Y軸を無視して距離を測る
            Vector3 enemyPos = new Vector3(transform.position.x, 0, transform.position.z);// ゾンビの位置を取得
            Vector3 playerPos = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);// プレイヤーの位置を取得
            float distanceToPlayer = Vector3.Distance(enemyPos, playerPos);// プレイヤーとゾンビの距離を計算

            // もしプレイヤーが攻撃範囲内にいる場合
            if (distanceToPlayer <= attackRange)
            {
                currentState = EnemyState.Attacking;
                return;
            }

            // もし視界内にプレイヤーがいる場合
            if (CanSeePlayer(distanceToPlayer))
            {
                currentState = EnemyState.Chasing;
            }
            else
            {
                currentState = EnemyState.Idle;
            }
        }

        /// <summary>
        /// 決定した状態に基づいてNavMeshAgentを動かす関数
        /// </summary>
        private void ExecuteAction()
        {
            // 状態に応じた行動を実行
            switch (currentState)
            {
                case EnemyState.Idle:
                    StopMovement();
                    break;

                case EnemyState.Chasing:
                    agent.isStopped = false;// 移動再開
                    agent.SetDestination(playerTransform.position);// 目的地をプレイヤーに設定
                    animator.SetFloat(walk_ID, canWalkValue);
                    break;

                case EnemyState.Attacking:

                    // もし攻撃中ではない場合
                    if (!isAttacking)
                    {
                        StartCoroutine(PerformAttackCoroutine());// 攻撃の一連の流れを管理するコルーチンを開始
                    }

                    break;
            }
        }

        /// <summary>
        /// プレイヤーが視界内にいるかどうかを判断する関数
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        private bool CanSeePlayer(float distance)
        {
            // もしプレイヤーが追いかける距離より遠い場合
            if (distance > chaseRange)
            {
                return false;// 遠すぎるので見えない
            }

            // 自分の正面方向とプレイヤーへの方向の角度を計算
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;// プレイヤーへの方向を正規化
            float angle = Vector3.Angle(transform.forward, directionToPlayer);// 自分の正面とプレイヤーへの方向の角度を計算

            // もし角度が視野角の範囲外の場合
            if (angle > detectionAngle)
            {
                return false;// 見えない
            }

            // もし壁などの障害物がある場合
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, distance, obstacleMask))
            {
                return false;// 壁があるので見えない
            }

            return true;// すべての条件を満たしているので見える
        }

        /// <summary>
        /// 攻撃の一連の流れを管理するコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator PerformAttackCoroutine()
        {
            isAttacking = true;

            StopMovement();

            // プレイヤーの方を向くための処理
            Vector3 lookPos = playerTransform.position;// プレイヤーの位置を取得
            lookPos.y = transform.position.y;// Y軸を固定してプレイヤーの方を向く
            transform.LookAt(lookPos);// プレイヤーの方を向く

            animator.SetTrigger(attack_ID);
            yield return new WaitForSeconds(attackBeforeTime);// 攻撃判定出現をアニメーションのタイミングと合うように調整
            attackCollider.enabled = true;// 攻撃コライダーを出現
            yield return new WaitForSeconds(attackTime);// 当たり判定を指定時間まで出現
            attackCollider.enabled = false;// 攻撃コライダーを消去
            yield return new WaitForSeconds(attackCooldown);// 攻撃のクールダウンを待つ

            isAttacking = false;
        }

        /// <summary>
        /// 動きを止めるための関数
        /// </summary>
        public void StopMovement()
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;// 物理的な勢いも消す
            animator.SetFloat(walk_ID, 0f);
        }

        /// <summary>
        /// 自身が死亡するための関数
        /// </summary>
        public void Die()
        {
            StartCoroutine(DieCoroutine());// 撃退演出を実行する
        }

        /// <summary>
        /// 撃退処理を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator DieCoroutine()
        {
            isDefeating = true;// 撃退演出を開始
            StopMovement();
            animator.SetTrigger(defeat_ID);// 敵の撃退アニメーション再生
            yield return new WaitForSeconds(defeatTime);// 演出中待機
            isDefeating = false;// 撃退演出を終了
            Destroy(this.gameObject);// 敵を消去
        }

        /// <summary>
        /// 被ダメージ演出の処理を行う関数
        /// </summary>
        public void Damage()
        {
            StartCoroutine(DamageCoroutine());// ダメージ処理を実行
        }

        /// <summary>
        /// ダメージ処理を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator DamageCoroutine()
        {
            isDamaging = true;// ダメージ演出を開始
            StopMovement();
            animator.SetTrigger(damage_ID);// 敵の被ダメージアニメーション再生
            yield return new WaitForSeconds(damageTime);// 演出中待機
            isDamaging = false;// ダメージ演出を終了
        }
    }
}
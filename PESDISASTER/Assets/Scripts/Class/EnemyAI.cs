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
        /// 歩きアニメーションのパラメーター値を参照する変数
        /// </summary>
        private float canWalkValue = 1f;
        /// <summary>
        /// 攻撃の連打を防ぐためのタイマーを参照する変数
        /// </summary>
        private float attackCooldown = 1.5f;
        /// <summary>
        /// 攻撃のクールダウンを管理するための最後の攻撃時間を参照する変数
        /// </summary>
        private float lastAttackTime;
        /// <summary>
        /// 気づいて追いかけ始める距離を参照する変数
        /// </summary>
        public float chaseRange = 15f;
        /// <summary>
        /// 攻撃が届く距離を参照する変数
        /// </summary>
        public float attackRange = 2f;

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
        /// ダメージアニメーションのパラメーターIDを参照する変数
        /// </summary>
        private static readonly int damage_ID = Animator.StringToHash("Damage");
        /// <summary>
        /// 敗北アニメーションのパラメーターIDを参照する変数
        /// </summary>
        private static readonly int defeat_ID = Animator.StringToHash("Defeat");

        // 敵の状態定義の列挙型
        public enum EnemyState
        {
            Idle,
            Chasing,
            Attacking
        }
        EnemyState currentState = EnemyState.Idle;// 初期状態は待機
        EnemyState lastState = EnemyState.Idle;// 1フレーム前の状態を保存する変数

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
            if (playerTransform == null)
            {
                return;
            }

            DetermineState();// 状態の決定

            ExecuteAction();// 状態に応じた行動の実行

            lastState = currentState;// 最後に現在の状態を保存
        }

        /// <summary>
        /// プレイヤーとの距離を測り、今の状態を決定する関数
        /// </summary>
        private void DetermineState()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);// プレイヤーとゾンビの距離を計算

            // もしプレイヤーが攻撃範囲内にいる場合
            if (distanceToPlayer <= attackRange)
            {
                currentState = EnemyState.Attacking;
            }
            // もしプレイヤーが追いかける範囲内にいる場合
            else if (distanceToPlayer <= chaseRange)
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

                    // もし待機状態に入った瞬間の場合
                    if (lastState != EnemyState.Idle)
                    {
                        agent.isStopped = true;// 移動停止
                        animator.SetFloat(walk_ID, 0f);// 歩きアニメーションを停止
                    }

                    break;

                case EnemyState.Chasing:
                    agent.isStopped = false;// 移動再開
                    agent.SetDestination(playerTransform.position);// 目的地をプレイヤーに設定
                    animator.SetFloat(walk_ID, canWalkValue);
                    break;

                case EnemyState.Attacking:
                    agent.isStopped = true; // 攻撃時は立ち止まる

                    // プレイヤーの方を向く処理
                    Vector3 lookPos = playerTransform.position;// プレイヤーの位置を取得
                    lookPos.y = transform.position.y;// 上下には傾かないようにする
                    transform.LookAt(lookPos);// プレイヤーの方を向く

                    // もし攻撃のクールダウンが終わっている場合
                    if (Time.time - lastAttackTime > attackCooldown)
                    {
                        animator.SetTrigger(attack_ID);// 攻撃アニメーションを再生
                        lastAttackTime = Time.time; // 最後に攻撃した時間を記録
                    }

                    break;
            }
        }
    }
}
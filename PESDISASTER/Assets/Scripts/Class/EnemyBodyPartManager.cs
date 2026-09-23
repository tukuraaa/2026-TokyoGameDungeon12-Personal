using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// エネミーの体部位判定を管理するクラス
    /// </summary>
    public class EnemyBodyPartManager : MonoBehaviour
    {
        /// <summary>
        /// 体力ステータス（エネミー）を管理するクラスを参照する変数
        /// </summary>
        [SerializeField]
        private HealthManager _enemyHealthManager;

        /// <summary>
        /// ヘッドショット時のダメージ倍率を参照する変数
        /// </summary>
        [SerializeField]
        private float _headshotMultiplier = 2.0f;

        /// <summary>
        /// 体部位の種類を参照する変数
        /// </summary>
        [SerializeField]
        private BodyPartType _bodyPartType;

        /// <summary>
        /// 調整後のダメージを参照する変数
        /// </summary>
        private float _changedDamage;

        /// <summary>
        /// 部位の種類を参照する列挙型変数
        /// </summary>
        public enum BodyPartType
        {
            Head,
            Body
        }

        /// <summary>
        /// ダメージ量を調整するクラス
        /// </summary>
        /// <param name="baseDamage">元のダメージ量を参照する変数</param>
        public void DamageValueChange(float baseDamage)
        {
            if (_enemyHealthManager == null)
            {
                return;
            }

            // 最初に元ダメージ量を変数に代入
            _changedDamage = baseDamage;

            switch (_bodyPartType)
            {
                case BodyPartType.Head:

                    // 代入されたダメージに部位別の倍率をかける
                    _changedDamage *= _headshotMultiplier;
                    Debug.Log("ヘッドショット！！");

                    break;

                case BodyPartType.Body:

                    // そのままのダメージ

                    break;
            }

            // 計算した最終的なダメージをエネミーに与える
            _enemyHealthManager.TakeDamage(_changedDamage);
        }
    }
}
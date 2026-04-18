using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// 錠前を銃で撃って破壊できるようにするクラス
    /// </summary>
    public class ShootableLock : MonoBehaviour
    {
        /// <summary>
        /// 連携する棚のスクリプトを参照する変数
        /// </summary>
        public LockedShelf targetShelf;

        /// <summary>
        /// 破壊時のエフェクトのプレハブを参照する変数
        /// </summary>
        public GameObject breakEffectPrefab;

        /// <summary>
        /// 銃弾が当たった時に呼ばれる関数
        /// </summary>
        public void BreakLock()
        {
            // もし錠前が連携する棚を持っている場合
            if (targetShelf != null)
            {
                targetShelf.Unlock();// 棚のロックを解除する
            }

            // もし破壊エフェクトのプレハブが設定されている場合
            if (breakEffectPrefab != null)
            {
                Instantiate(breakEffectPrefab, transform.position, transform.rotation);// 錠前の位置と回転でエフェクトを生成する
            }

            Destroy(gameObject);// 3. 錠前自体を消滅させる
        }
    }
}
using UnityEngine;
using System.Collections;

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
        /// 錠前のオブジェクトを参照する変数
        /// </summary>
        public GameObject padlockObject;

        /// <summary>
        /// 錠前が破壊されたかどうかを管理する変数
        /// </summary>
        public bool isBroken = false;

        /// <summary>
        /// 壊れる演出の時間を参照する変数
        /// </summary>
        private float breakEffectDuration = 0.5f;

        /// <summary>
        /// 銃弾が当たった時に呼ばれるコルーチン
        /// </summary>
        public IEnumerator BreakLockCoroutine()
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

            Destroy(padlockObject);// 錠前自体を消滅させる

            yield return new WaitForSeconds(breakEffectDuration);// 壊れる演出の時間を待つ

            isBroken = true;
        }
    }
}
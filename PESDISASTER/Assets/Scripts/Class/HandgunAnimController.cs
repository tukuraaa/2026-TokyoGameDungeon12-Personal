using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// ハンドガンのアニメーションイベントを処理するクラス
    /// </summary>
    public class HandgunAnimController : MonoBehaviour
    {
        /// <summary>
        /// マズルフラッシュの親パーティクルシステムを参照する変数
        /// </summary>
        public ParticleSystem muzzleFlashParent;
        /// <summary>
        /// マズルフラッシュの子パーティクルシステムを参照する変数
        /// </summary>
        public ParticleSystem muzzleFlashChild;

        /// <summary>
        /// アニメーションイベント'Shoot'の受け皿の関数
        /// </summary>
        public void Shoot()
        {
            if (muzzleFlashChild != null && muzzleFlashParent != null)
            {
                // マズルフラッシュの親エフェクトが光る
                muzzleFlashParent.Stop();
                muzzleFlashParent.Play();

                // マズルフラッシュの子エフェクトが光る
                muzzleFlashChild.Stop();
                muzzleFlashChild.Play();
            }

            // ここに発砲音やレイキャストの処理を書く

            Debug.Log("Bang!");
        }

        /// <summary>
        /// アニメーションイベント'CasingRelease'の受け皿の関数
        /// </summary>
        public void CasingRelease()
        {
            // 排莢（薬莢が飛び出す）エフェクトなどがあればここに書く
        }
    }
}
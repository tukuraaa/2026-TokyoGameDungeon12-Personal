using UnityEngine;
using UnityEngine.Events;

namespace PESDISASTER
{
    /// <summary>
    /// ゴールの判定を管理するクラス
    /// </summary>
    public class Goal : MonoBehaviour
    {
        /// <summary>
        /// トリガー内に侵入した際に発生すUnityEventを取得した変数
        /// </summary>
        public UnityEvent getOnEnter { get => onEnter; set => onEnter = value; }
        /// <summary>
        /// トリガー内に侵入した際に発生するUnityEventを参照する変数
        /// </summary>
        public UnityEvent onEnter = null;

        /// <summary>
        /// プレイヤーのタグを参照する変数
        /// </summary>
        private string playerTag = "Player";

        /// <summary>
        /// トリガー内に他のオブジェクトが侵入してきた際に呼び出される関数
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter(Collider collision)
        {
            // もし触れたコライダーにプレイヤータグが付いていた場合
            if (collision.CompareTag(playerTag))
            {
                getOnEnter.Invoke();// イベント起動
            }
        }
    }
}
using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// アイテムを拾うためのクラス
    /// </summary>
    public class PickupItem : MonoBehaviour, IInteractable
    {
        /// <summary>
        /// アイテムの名前の変数
        /// </summary>
        [SerializeField]
        public string itemName = "ハンドガン";

        /// <summary>
        /// 拾うときの処理を行う関数
        /// </summary>
        public void Interact()
        {
            Destroy(gameObject);// シーン上からアイテムを消す
        }

        /// <summary>
        /// 拾ったアイテムの名前を表示する関数
        /// </summary>
        /// <returns></returns>
        public string GetInteractText()
        {
            return $"{itemName} を拾う";// 拾ったアイテムの名前を表示する
        }
    }
}
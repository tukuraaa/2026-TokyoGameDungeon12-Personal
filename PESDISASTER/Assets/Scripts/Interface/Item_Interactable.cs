using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// インタラクト可能なオブジェクトが実装するインターフェース
    /// </summary>
    public interface Item_Interactable
    {
        /// <summary>
        /// インタラクトしたときの処理を行う関数
        /// </summary>
        public void Pickup(string itemName);
    }
}
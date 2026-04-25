using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// インタラクト可能なオブジェクトが実装するインターフェース
    /// </summary>
    public interface D_Interactable
    {
        /// <summary>
        /// プレイヤーがドアにアクセスした時に呼ばれる関数
        /// </summary>
        public void Interact();
    }
}
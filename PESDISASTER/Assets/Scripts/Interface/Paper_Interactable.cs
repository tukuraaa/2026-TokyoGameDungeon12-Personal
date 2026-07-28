namespace PESDISASTER
{
    /// <summary>
    /// インタラクト可能なオブジェクトが実装するインターフェース
    /// </summary>
    public interface Paper_Interactable
    {
        /// <summary>
        /// プレイヤーがメモにアクセスした時に呼ばれる関数
        /// </summary>
        public void Interact();
    }
}
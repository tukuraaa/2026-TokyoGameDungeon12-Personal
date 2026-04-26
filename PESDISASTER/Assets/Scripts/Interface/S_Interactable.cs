namespace PESDISASTER
{
    /// <summary>
    /// インタラクト可能なオブジェクト（棚）が実装するインターフェース
    /// </summary>
    public interface S_Interactable
    {
        /// <summary>
        /// プレイヤーが棚にアクセスした時に呼ばれる関数
        /// </summary>
        public void OpenShelf();
    }
}
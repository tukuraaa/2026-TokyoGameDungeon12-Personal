namespace PESDISASTER
{
    /// <summary>
    /// インタラクト可能なオブジェクトが実装するインターフェース
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// インタラクトしたときの処理を行う関数
        /// </summary>
        public void Pickup();

        /// <summary>
        /// アイテムの説明文を返す関数
        /// </summary>
        /// <returns></returns>
        public string GetInteractText();
    }
}
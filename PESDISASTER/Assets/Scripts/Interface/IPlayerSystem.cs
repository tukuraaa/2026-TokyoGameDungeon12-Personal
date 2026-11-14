using System;

namespace PESDISASTER
{
    /// <summary>
    /// プレイヤー関連のシステム機能を管理するインターフェースクラス
    /// </summary>
    public interface IPlayerSystem
    {
        /// <summary>
        /// システム機能を呼び出すイベントを行う変数
        /// </summary>
        public event Action OnInvokeSystem;
    }
}
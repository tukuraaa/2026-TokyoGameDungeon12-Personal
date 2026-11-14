using System;
using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// UIの表示を管理するインターフェースクラス
    /// </summary>
    public interface IUIViewer
    {
        /// <summary>
        /// UIを表示するイベントを行う変数
        /// </summary>
        public event Action<Transform,bool> OnSetActiveUI;
    }
}
using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PESDISASTER
{
    /// <summary>
    /// メモUIを管理するクラス
    /// </summary>
    public class PaperObjectManager : MonoBehaviour, PaperInteractable
    {
        /// <summary>
        /// バリア管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private BarrierManager _barrierManager;

        /// <summary>
        /// 現在のメッセージID名を参照する変数
        /// </summary>
        [SerializeField]
        private string _messageIDName;
        
        /// <summary>
        /// プレイヤーがメモにアクセスした時に呼ばれる関数
        /// </summary>
        public void Interact()
        {
            // メモ内容を表示するイベント処理を呼び出し
            PaperUIManager.Instance.ShowPaper(_barrierManager, _messageIDName);
        }
    }
}
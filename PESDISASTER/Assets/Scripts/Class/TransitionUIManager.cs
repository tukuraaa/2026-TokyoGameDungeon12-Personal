using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// 遷移用UIを管理するクラス
    /// </summary>
    public class TransitionUIManager : MonoBehaviour
    {
        public static TransitionUIManager Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else
            {
                this.enabled = false;
                Destroy(gameObject);
                return;
            }
        }
    }
}
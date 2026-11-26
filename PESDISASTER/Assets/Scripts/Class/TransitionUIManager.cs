using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// 遷移用UIを管理するクラス
    /// </summary>
    public class TransitionUIManager : MonoBehaviour
    {
        /// <summary>
        /// このクラスのシングルトンインスタンス
        /// </summary>
        public static TransitionUIManager Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// 初期設定を行うメソッド
        /// </summary>
        private void Awake()
        {
            if (Instance == null) 
            {
                Instance = this; 
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// プレイヤー通知UIを管理するクラス
    /// </summary>
    public class UIGeneralManager : MonoBehaviour
    {
        /// <summary>
        /// クラス自身のシングルトンインスタンスを参照する変数
        /// </summary>
        public static UIGeneralManager Instance { get; private set; }

        /// <summary>
        /// 初期設定を行う関数
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

        /// <summary>
        /// UIを表示・非表示にする関数
        /// </summary>
        /// <param name="isActive">表示・非表示を指定するフラグ変数</param>
        /// <param name="target">表示・非表示の対象となるオブジェクト変数</param>
        public void UIShowHide(bool isActive, Transform target)
        {
            // 対象UIの全子オブジェクトをサーチ
            foreach (Transform child in target.transform)
            {
                child.gameObject.SetActive(isActive);
            }
        }
    }
}
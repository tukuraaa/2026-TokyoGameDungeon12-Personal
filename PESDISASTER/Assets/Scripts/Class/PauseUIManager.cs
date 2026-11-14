using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// ポーズUIを管理するクラス
    /// </summary>
    public class PauseUIManager : MonoBehaviour
    {
        /// <summary>
        /// UI表示を行うインターフェースを参照する変数
        /// </summary>
        private IUIViewer _uiViewerSource;

        /// <summary>
        /// イベント登録の初期設定を行う関数
        /// </summary>
        /// <param name="source">インターフェースのソースを参照する変数</param>
        public void Init(IUIViewer source)
        {
            _uiViewerSource = source;
            _uiViewerSource.OnSetActiveUI += SetActivePauseUI;
        }

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // 最初は演出など抜きでUIを非表示
            UIGeneralManager.Instance.SetActiveChildUI(this.transform, false);
        }

        /// <summary>
        /// ポーズUIを表示・非表示する際の処理・演出を行う関数
        /// </summary>
        /// <param name="transform">対象の座標を参照する変数</param>
        /// <param name="isActive">表示・非表示を判別するフラグ</param>
        public void SetActivePauseUI(Transform transform, bool isActive)
        { 
            UIGeneralManager.Instance.SetActiveChildUI(transform, isActive);
        }
    }
}
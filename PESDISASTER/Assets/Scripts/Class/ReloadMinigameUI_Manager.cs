using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PESDISASTER
{
    /// <summary>
    /// リロードミニゲームのUIを管理するクラス
    /// </summary>
    public class ReloadMinigameUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// 画像ごとの個別設定を保存するための構造体
        /// </summary>
        [Serializable]
        public struct PromptUIData
        {
            /// <summary>
            /// 表示する画像を参照する変数
            /// </summary>
            public Sprite Control_Sprite;

            /// <summary>
            /// サイズ調整後の追加のスケール倍率を参照する変数
            /// </summary>
            public Vector3 ExtraScale;

            /// <summary>
            /// このUIが対応する操作の名前を参照する変数
            /// </summary>
            public string Control_Name;

            /// <summary>
            /// 画像の表示幅を参照する変数
            /// </summary>
            public float TargetWidth;
        }

        /// <summary>
        /// UI設定のリストを参照する変数
        /// </summary>
        [SerializeField]
        private List<PromptUIData> _uI_SettingsList = new List<PromptUIData>();

        /// <summary>
        /// キーを表示する画像を参照する変数
        /// </summary>
        [SerializeField]
        private Image _prompt_Image;

        /// <summary>
        /// リロードガンモデルの管理クラスを参照する変数
        /// </summary>
      public ReloadGunModel_Manager ReloadGunModel_Manager;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // UIを隠す
            Hide();
        }

        /// <summary>
        /// UIを非表示にする関数
        /// </summary>
        public void Hide()
        {
            // 子オブジェクトを全てチェック
            foreach (Transform _child in transform)
            {
                // 子オブジェクトを非表示
                _child.gameObject.SetActive(false);
            }

            // リロードガンモデルのUIも非表示にする
            ReloadGunModel_Manager.Hide();
        }

        /// <summary>
        /// UIを表示する関数
        /// </summary>
        public void Show()
        {
            // 子オブジェクトを全てチェック
            foreach (Transform _child in transform)
            {
                // 子オブジェクトを表示
                _child.gameObject.SetActive(true);
            }

            // リロードガンモデルのUIも表示する
            ReloadGunModel_Manager.TargetShow(ReloadGunModel_Manager.Handgun);
        }

        /// <summary>
        /// 現在のステップに合わせてUI画像の情報を更新する関数
        /// </summary>
        public void UpdateUI(string _control_Name)
        {
            // リストの中から_control_Nameが一致するデータを検索
            PromptUIData _data = _uI_SettingsList.Find(_x => _x.Control_Name == _control_Name);

            // もしデータが見つからなかった場合
            if (string.IsNullOrEmpty(_data.Control_Name))
            {
                return;
            }

            // 画像の反映
            _prompt_Image.sprite = _data.Control_Sprite;

            // 元のサイズ（Native Size）に適応
            _prompt_Image.SetNativeSize();

            // 指定された横幅をベースに、縦横比を崩さずサイズ変更
            RectTransform _rectTransform = _prompt_Image.rectTransform;

            // もし指定された横幅が0より大きい場合
            if (_data.TargetWidth > 0f)
            {
                // 現在の画像本来の縦横比を計算
                float _aspectRatio = _rectTransform.sizeDelta.y / _rectTransform.sizeDelta.x;

                // 指定された横幅と、比率から計算した縦幅をセットする
                _rectTransform.sizeDelta = new Vector2(_data.TargetWidth, _data.TargetWidth * _aspectRatio);
            }

            // もし追加したスケールが0の場合
            if (_data.ExtraScale == Vector3.zero)
            {
                // スケールを1にリセット
                _rectTransform.localScale = Vector3.one;
            }
            else
            {
                // 追加のスケールを適用
                _rectTransform.localScale = _data.ExtraScale;
            }
        }
    }
}
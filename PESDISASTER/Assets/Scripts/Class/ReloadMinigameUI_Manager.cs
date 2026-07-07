using System;
using System.Collections;
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
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// リロードパネル表示演出トリガーを参照する変数
        /// </summary>
        private static readonly int _reloadStartTrigger_ID = Animator.StringToHash("OnReloadStart");
        /// <summary>
        /// リロードパネル表示演出トリガーを参照する変数
        /// </summary>
        private static readonly int _reloadEndTrigger_ID = Animator.StringToHash("OnReloadEnd");

        /// <summary>
        /// リロードUI非表示演出時間を参照する変数
        /// </summary>
        private float _reloadPanel_HideTime = 1;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // --- コンポーネントの登録 ---
            _animator = GetComponent<Animator>();

            // UIを隠す
            Hide(false);
        }

        /// <summary>
        /// UIを非表示にする関数
        /// </summary>
        public void Hide(bool isAnim)
        {
            // もしアニメーションフラグがオンの場合
            if (isAnim)
            {
                // 非表示演出を行うコルーチンを呼び出し
                StartCoroutine(HideAnimCoroutine());

                return;
            }

            // 子オブジェクトを全てチェック
            foreach (Transform _child in transform)
            {
                // 子オブジェクトを非表示
                _child.gameObject.SetActive(false);
            }
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

            // パネル表示演出再生
            _animator.SetTrigger(_reloadStartTrigger_ID);
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

        /// <summary>
        /// UIを非表示にする処理・演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator HideAnimCoroutine()
        {
            // 非表示演出を再生
            _animator.SetTrigger(_reloadEndTrigger_ID);
            // 演出時間分待機
        yield return new WaitForSeconds(_reloadPanel_HideTime);

            // 子オブジェクトを全てチェック
            foreach (Transform _child in transform)
            {
                // 子オブジェクトを非表示
                _child.gameObject.SetActive(false);
            }
        }
    }
}
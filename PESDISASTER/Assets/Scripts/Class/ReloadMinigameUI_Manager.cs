using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PESDISASTER
{
    /// <summary>
    /// リロードミニゲームのUIを管理するクラス
    /// </summary>
    public class ReloadMinigameUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// キーを表示するテキストを参照する変数
        /// </summary>
        public TextMeshProUGUI promptText;

        /// <summary>
        /// 成否を伝えるためのコールバックを参照する変数
        /// </summary>
        private Action<bool> onComplete;

        /// <summary>
        /// アクションが有効かどうかを示すフラグ
        /// </summary>
        private bool isActive = false;

        /// <summary>
        /// ターゲットのキーを参照する変数
        /// </summary>
        private Key targetKey;
        /// <summary>
        /// ミニゲームで使用するキーの候補を参照する変数の配列
        /// </summary>
        private Key[] possibleKeys = { Key.E, Key.F, Key.Q, Key.T };

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        void Start()
        {
            Hide();
        }

        /// <summary>
        /// UIを非表示にする関数
        /// </summary>
        private void Hide()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// リロードミニゲームを開始する関数
        /// </summary>
        /// <param name="callback"></param>
        public void StartMinigame(Action<bool> callback)
        {
            // もしすでにミニゲームがアクティブの場合
            if (isActive)
            {
                return;
            }

            onComplete = callback;// コールバックを保存
            isActive = true;
            Show();

            // ランダムにキーを選択
            targetKey = possibleKeys[UnityEngine.Random.Range(0, possibleKeys.Length)];// ランダムにキーを選ぶ
            promptText.text = targetKey.ToString();// 指定のキーが表示される
        }

        /// <summary>
        /// 毎フレーム処理を行う関数
        /// </summary>
        private void Update()
        {
            // もしミニゲームがアクティブでない場合
            if (!isActive)
            {
                return;
            }

            // もし正解のキーが押された場合
            if (Keyboard.current[targetKey].wasPressedThisFrame)
            {
                Finish(true);// 正解のキーが押された場合は成功
                return;
            }

            // 特定のキー候補をすべてチェックする
            foreach (var key in possibleKeys)
            {
                // もし不正解のキーが押された場合
                if (key != targetKey && Keyboard.current[key].wasPressedThisFrame)
                {
                    Finish(false);// 不正解のキーが押された場合は失敗
                    return;
                }
            }
        }

        /// <summary>
        /// 成功または失敗を処理してミニゲームを終了する関数
        /// </summary>
        /// <param name="success"></param>
        private void Finish(bool success)
        {
            isActive = false;
            Hide();
            onComplete?.Invoke(success);// ハンドガン側に成否を伝える
        }

        /// <summary>
        /// UIを表示する関数
        /// </summary>
        private void Show()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
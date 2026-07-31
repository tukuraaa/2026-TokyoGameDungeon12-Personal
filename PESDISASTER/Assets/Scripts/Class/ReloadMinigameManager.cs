using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PESDISASTER
{
    /// <summary>
    /// リロードミニゲームのUIを管理するクラス
    /// </summary>
    public class ReloadMinigameManager : MonoBehaviour
    {
        /// <summary>
        /// リロードミニゲームのUIを管理するクラスを参照する変数
        /// </summary>
        [SerializeField]
        private ReloadMinigameUI_Manager _reloadMinigameUI_Manager;

        /// <summary>
        /// マウスドラッグ距離のしきい値を参照する変数
        /// </summary>
        [SerializeField]
        private float _dragDistanceThreshold = 100f;

        /// <summary>
        /// 成否を伝えるためのコールバックを参照する変数
        /// </summary>
        private Action<bool> _onComplete;

        /// <summary>
        /// アクションが有効かどうかを示すフラグを参照する変数
        /// </summary>
        private bool _isActive = false;
        /// <summary>
        /// マウスドラッグ中かどうかを示すフラグを参照する変数
        /// </summary>
        private bool _isDragging = false;
        /// <summary>
        /// 開始フレーム判定用フラグを参照する変数
        /// </summary>
        private bool _isFirstFrame = false;
        /// <summary>
        /// リロードコマンド指定が右クリックの時かを判別するフラグを参照する変数
        /// </summary>
        private bool _isCheckRightClick = false;
        /// <summary>
        /// リロードコマンド指定がマウスドラッグ下の時かを判別するフラグを参照する変数
        /// </summary>
        private bool _isCheckDragDown = false;

        /// <summary>
        /// マウスドラッグの下方向への累積移動距離を参照する変数
        /// </summary>
        private float _accumulatedDragDistance = 0f;
        /// <summary>
        /// 銃を普段の手元位置に戻す演出時間を参照する変数
        /// </summary>
        private float _reloadGunEventAnimTime = 0.2f;
        /// <summary>
        /// 銃の最終段階演出時間を参照する変数
        /// </summary>
        private float _lastReloadTime = 1.0f;

        /// <summary>
        /// リロードのステップを管理する列挙型
        /// </summary>
        private enum ReloadStep
        {
            RightClick,
            PressT,
            PressR,
            DragDown
        }
        /// <summary>
        /// 現在のステップ状態を参照する変数
        /// </summary>
        private ReloadStep _currentStep;

        /// <summary>
        /// リロードミニゲームを開始する関数
        /// </summary>
        /// <param name="callback"></param>
        public void StartMinigame(Action<bool> callback)
        {
            // もしすでにミニゲームがアクティブの場合
            if (_isActive)
            {
                return;
            }

            // --- ミニゲーム前の準備 ---
            // コールバックを保存
            _onComplete = callback;
            // ミニゲーム開始フラグをオン
            _isActive = true;
            // ドラッグ状態フラグをオフ
            _isDragging = false;
            // 開始時フラグをオン
            _isFirstFrame = true;
            // ハンドガンのリロード中フラグをオン
            HandgunController.Instance.IsReloading = true;
            // ハンドガンのエイムフラグをリセット
            HandgunController.Instance.IsAiming = false;
            // プレイヤーの視点移動の力をリセット
            PlayerController.Instance.Look_Input = Vector2.zero;

            // 最初のステップを設定
            _currentStep = ReloadStep.RightClick;

            // --- UIの表示物を指定して表示 ---
            _reloadMinigameUI_Manager.UpdateUI("RightClick");
            _reloadMinigameUI_Manager.Show();

            // ハンドガンを手元に寄せる演出再生
            HandgunController.Instance.ReloadMotion(HandgunController.Handgun_IntroTrigger_ID, null);
        }

        /// <summary>
        /// 毎フレーム処理を行う関数
        /// </summary>
        private void Update()
        {
            // もしミニゲームがアクティブでない場合
            if (!_isActive)
            {
                return;
            }

            // --- 開始したフレーム（Rキーが押された瞬間）は入力を無視して次のフレームへ ---
            // もしミニゲーム開始時だった場合
            if (_isFirstFrame)
            {
                // ミニゲーム開始時フラグをオフ
                _isFirstFrame = false;

                return;
            }

            // もしマウス、もしくはキーボードの接続が無い場合
            if (Mouse.current == null || Keyboard.current == null)
            {
                return;
            }

            // 入力状態ごとの分岐
            switch (_currentStep)
            {
                case ReloadStep.RightClick:

                    // 入力判定を開始
                    CheckRightClick();

                    break;

                case ReloadStep.PressT:

                    // 入力判定を開始
                    CheckKeyInput(Keyboard.current.tKey, null, ReloadStep.PressR, "PressR", HandgunController.HandgunStage2_Trigger_ID);

                    break;

                case ReloadStep.PressR:

                    // 入力判定を開始
                    CheckKeyInput(Keyboard.current.rKey, HandgunController.Instance.Full_MagazineModel, ReloadStep.DragDown, "DragDown", HandgunController.HandgunStage3_Trigger_ID);

                    break;

                case ReloadStep.DragDown:

                    // 入力判定を開始
                    CheckDragDown();

                    break;
            }
        }

        /// <summary>
        /// 右クリックの判定をする関数
        /// </summary>
        private void CheckRightClick()
        {
            // 右クリック指定フラグをオン
            _isCheckRightClick = true;

            // もしマウスの右クリックを行った場合
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                // ハンドガンのリロードモーションを再生
                HandgunController.Instance.ReloadMotion(HandgunController.HandgunStage1_Trigger_ID, "Reload");
                // 次の状態へ
                ProceedToNextStep(ReloadStep.PressT, "PressT");
            }
            // 他の入力をした場合
            else if (IsAnyWrongInputPressed())
            {
                // 間違った入力で失敗
                Finish(false);
            }

            // 右クリック指定フラグをオフ
            _isCheckRightClick = false;
        }

        /// <summary>
        /// 特定のキー入力の判定をする関数
        /// </summary>
        /// <param name="targetKey"></param>
        /// <param name="addMagazineModel"></param>
        /// <param name="nextStep"></param>
        /// <param name="control_Name"></param>
        /// <param name="trigger_ID"></param>
        private void CheckKeyInput(UnityEngine.InputSystem.Controls.KeyControl targetKey, GameObject addMagazineModel, ReloadStep nextStep, string control_Name, int trigger_ID)
        {
            // もし指定のキーを入力した場合
            if (targetKey.wasPressedThisFrame)
            {
                // 指定のマガジンモデルを変更
                HandgunController.Instance.ChangeMagazine(addMagazineModel, "Hold_Item");
                // ハンドガンのリロードモーションを再生
                HandgunController.Instance.ReloadMotion(trigger_ID, "Reload");
                // 次の状態へ
                ProceedToNextStep(nextStep, control_Name);
            }
            // 他の入力をした場合
            else if (IsAnyWrongInputPressed())
            {
                // 間違った入力で失敗
                Finish(false);
            }
        }

        /// <summary>
        /// 下方向へのドラッグ判定をする関数
        /// </summary>
        private void CheckDragDown()
        {
            // マウスドラッグ下指定フラグをオン
            _isCheckDragDown = true;

            // もしドラッグ中以外の時に間違ったキーが押された場合
            if (!_isDragging && IsAnyWrongInputPressed())
            {
                // 間違った入力で失敗
                Finish(false);

                return;
            }

            // もし左クリックした場合
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // --- その瞬間に開始位置を記録 ---
                // ドラッグのフラグをオン
                _isDragging = true;
                // 累積移動距離をリセットする
                _accumulatedDragDistance = 0f;
            }

            // もしドラッグ中の場合
            if (_isDragging)
            {
                // 「下」への移動量をプラスの距離として測りたいので、マイナスを掛けて加算
                _accumulatedDragDistance -= Mouse.current.delta.y.ReadValue();
            }

            // もし左クリックを離した場合
            if (_isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                // --- その瞬間に距離を計算 ---
                // ドラッグのフラグをオフ
                _isDragging = false;

                // もしマウスドラッグ距離のしきい値以上に下方向への移動数が多い場合
                if (_accumulatedDragDistance >= _dragDistanceThreshold)
                {
                    // 全リロードステップ成功時の処理を行うコルーチンを呼び出し
                    StartCoroutine(ReloadCompleteCoroutine(HandgunController.HandgunStage4_Trigger_ID));
                }
                else
                {
                    // ドラッグ距離が足りない、または上方向にドラッグした場合は失敗
                    Finish(false);
                }
            }

            // マウスドラッグ下指定フラグをオフ
            _isCheckDragDown = false;
        }

        /// <summary>
        /// 次のステップへ進む関数
        /// </summary>
        private void ProceedToNextStep(ReloadStep nextStep, string control_Name)
        {
            // 次の状態へステートを変更
            _currentStep = nextStep;
            // UIの画像を切り替える
            _reloadMinigameUI_Manager.UpdateUI(control_Name);
        }

        /// <summary>
        /// 指定された操作以外が行われたかを判定する関数
        /// </summary>
        private bool IsAnyWrongInputPressed()
        {
            // --- 期待されている操作以外で何かしらのキーやクリックが押されたらtrueを返す ---
            // もし何かしらのキーが押された場合
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                // 除外するキー（W, A, S, D, ESC）のいずれかが押された場合は、ここではエラーにしない
                if (Keyboard.current.wKey.wasPressedThisFrame ||
                    Keyboard.current.aKey.wasPressedThisFrame ||
                    Keyboard.current.sKey.wasPressedThisFrame ||
                    Keyboard.current.dKey.wasPressedThisFrame ||
                    Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    // 間違った判定は行わない
                    return false;
                }
                else
                {
                    // 除外キー以外の関係ないキーが押されたので、間違った操作として判定する
                    return true;
                }
            }
            // もし関係のないときにマウスの左クリックをした場合
            if (!_isCheckDragDown && Mouse.current.leftButton.wasPressedThisFrame)
            {
                // 間違った操作をした
                return true;
            }
            // もし関係のないときにマウスの右クリックをした場合
            if (!_isCheckRightClick && Mouse.current.rightButton.wasPressedThisFrame)
            {
                // 間違った操作をした
                return true;
            }
            // もし関係のないときにマウスの真ん中クリックをした場合
            if (Mouse.current.middleButton.wasPressedThisFrame)
            {
                // 間違った操作をした
                return true;
            }

            // 正しい操作をした
            return false;
        }

        /// <summary>
        /// 成功または失敗を処理してミニゲームを終了する関数
        /// </summary>
        /// <param name="isSuccess"></param>
        private void Finish(bool isSuccess)
        {
            // 処理を行うコルーチンを呼び出し
            StartCoroutine(FinishEventCoroutine(isSuccess));
        }

        /// <summary>
        /// ミニゲーム終了時の処理を行うコルーチン
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        private IEnumerator FinishEventCoroutine(bool isSuccess)
        {
            // ドラッグフラグをオフ
            _isDragging = false;
            // ミニゲーム稼働中フラグをオフ
            _isActive = false;
            // ミニゲームのUIを非表示
            _reloadMinigameUI_Manager.Hide(true);
            // ハンドガンを手元に寄せる演出再生
            HandgunController.Instance.ReloadMotion(HandgunController.HandgunOutroTrigger_ID, "Reload");
            // 演出分待機
            yield return new WaitForSeconds(_reloadGunEventAnimTime);
            // ハンドガンのリロード中フラグをオフ
            HandgunController.Instance.IsReloading = false;
            // ハンドガン側に成否を伝える
            _onComplete?.Invoke(isSuccess);
        }

        /// <summary>
        /// リロードの工程が成功したときの処理を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReloadCompleteCoroutine(int trigger_ID)
        {
            // 指定のリロードモーション演出再生
            HandgunController.Instance.ReloadMotion(trigger_ID, "Reload");
            // 演出分待機
            yield return new WaitForSeconds(_lastReloadTime);
            // ミニゲームクリア処理を呼び出し
            Finish(true);
        }
    }
}
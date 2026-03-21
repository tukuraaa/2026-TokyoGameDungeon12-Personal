using UnityEngine;
using UnityEngine.InputSystem;

namespace PESDISASTER
{
    /// <summary>
    /// プレイヤーの操作を管理するクラス
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// カメラの参照用変数
        /// </summary>
        public Camera mainCamera = null;

        /// <summary>
        /// レイヤーマスクを使用して、インタラクト可能なオブジェクトを特定するための変数
        /// </summary>
        public LayerMask interactableLayer = default;

        /// <summary>
        /// 首のTransformコンポーネントへの参照用変数
        /// </summary>
        public Transform neck = null;

        /// <summary>
        /// 首の前後移動の入力を保持するための変数
        /// </summary>
        private float translationZ = 0f;
        /// <summary>
        /// 回転角度を保持するための変数
        /// </summary>
        private float rotationX = 0f;
        /// <summary>
        /// マウス感度を調整するための変数
        /// </summary>
        public float sensitivity = 2.0f;
        /// <summary>
        /// 首の上下回転の制限角度を設定するための変数
        /// </summary>
        public float minVertical = -90.0f;
        /// <summary>
        /// 最大の首の上下回転角度を設定するための変数
        /// </summary>
        public float maxVertical = 90.0f;
        /// <summary>
        /// 首の前後移動の最小値を設定するための変数
        /// </summary>
        public float minNeckTranslationZ = -1.0f;
        /// <summary>
        /// 首の前後移動の最大値を設定するための変数
        /// </summary>
        public float maxNeckTranslationZ = 1.0f;
        /// <summary>
        /// 首の前後移動の感度を調整するための変数
        /// </summary>
        public float adjustmentDivisor = 200.0f;
        /// <summary>
        /// インタラクト可能なオブジェクトを検出するための範囲を設定する変数
        /// </summary>
        public float interactRange = 2.5f;

        /// <summary>
        /// 視点移動の入力を保持するための変数
        /// </summary>
        private Vector2 lookInput = Vector2.zero;

        /// <summary>
        /// ターゲットとなるインタラクト可能なオブジェクトを保持するための変数
        /// </summary>
        private IInteractable currentTarget = null;

        /// <summary>
        /// ゲーム開始時の初期設定を行う関数
        /// </summary>
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;// カーソルを非表示にする
            currentTarget = GetComponent<IInteractable>();
        }

        /// <summary>
        /// 毎フレーム呼び出される関数
        /// </summary>
        private void Update()
        {
            // マウスの入力を感度とフレーム時間で調整して、回転と移動の値を更新
            float mouseRotationX = lookInput.x * sensitivity * Time.deltaTime;// マウスX方向の入力を感度とフレーム時間で調整
            float mouseRotationY = lookInput.y * sensitivity * Time.deltaTime;// マウスY方向の入力を感度とフレーム時間で調整

            float mouseTranslationY = lookInput.y * (sensitivity / adjustmentDivisor) * Time.deltaTime;// マウスY方向の入力を感度とフレーム時間で調整して、首の前後移動の値を更新

            float neckTranslationY = neck.transform.localPosition.y;// 首の高さを保持するために現在の首のY位置を取得

            transform.Rotate(0, mouseRotationX, 0);// プレイヤー（体）の左右の回転をマウスX方向の入力に合わせて行う

            // 首の回転と前後移動をマウスY方向の入力に合わせて更新
            rotationX -= mouseRotationY;// マウスY方向の入力によって縦方向の回転を更新
            translationZ = Mathf.Clamp(translationZ, minNeckTranslationZ, maxNeckTranslationZ);// 首の前後移動を指定された範囲に制限
            neck.localRotation = Quaternion.Euler(rotationX, 0, 0);// 首の回転を設定。縦方向のみ回転させる

            // 首の前後移動をマウスY方向の入力に合わせて更新
            translationZ -= mouseTranslationY;// マウスY方向の入力によって首の前後移動を更新
            rotationX = Mathf.Clamp(rotationX, minVertical, maxVertical);// 回転角度を指定された範囲に制限
            neck.localPosition = new Vector3(0, 0, translationZ);// 首の位置を設定。前後移動のみ行う

            neck.localPosition = new Vector3(neck.localPosition.x, neckTranslationY, neck.localPosition.z);// 首の高さを一定に保つ

            CheckForInteractable();// インタラクト可能なオブジェクトを検出する関数を呼び出す
        }

        /// <summary>
        /// 視点移動の入力を処理する関数
        /// </summary>
        /// <param name="context"></param>
        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();// 視点移動の入力を取得
        }

        /// <summary>
        /// アイテムを拾うための入力を処理する関数
        /// </summary>
        /// <param name="context"></param>
        public void OnInteract(InputAction.CallbackContext context)
        {
            // もしインタラクトの入力が開始され、ターゲットが存在する場合
            if (context.started && currentTarget != null)
            {
                currentTarget.Pickup();// ターゲットにインタラクト関数を呼び出す
            }
        }

        /// <summary>
        /// インタラクト可能なオブジェクトを検出する関数
        /// </summary>
        private void CheckForInteractable()
        {
            // 画面中央から奥へ向かうRayを作成
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));// 画面の中心から奥へ向かうRayを作成
            RaycastHit hit;// Rayが当たった情報を格納する変数

            // もしRayがインタラクト可能なオブジェクトに当たった場合
            if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();// Rayが当たったオブジェクトにIInteractableコンポーネントがあるか確認

                // もしIInteractableコンポーネントがある場合
                if (interactable != null)
                {
                    currentTarget = interactable;// ターゲットを更新
                    return;
                }
            }

            currentTarget = null;
        }
    }
}
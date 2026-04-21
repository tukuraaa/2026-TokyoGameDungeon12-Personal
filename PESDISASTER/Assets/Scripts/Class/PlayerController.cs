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
        /// プレイヤー視点カメラの参照用変数
        /// </summary>
        public Camera mainCamera;
        /// <summary>
        /// アイテム表示カメラの参照用変数
        /// </summary>
        public Camera weaponCamera;

        /// <summary>
        /// 首のTransformコンポーネントへの参照用変数
        /// </summary>
        public Transform neck;
        /// <summary>
        /// カメラ右手元の位置を参照する変数
        /// </summary>
        public Transform holdPosition;
        /// <summary>
        /// プレイヤー視点カメラのTransformコンポーネントを参照する変数
        /// </summary>
        public Transform mainCameraTransform;
        /// <summary>
        /// アイテム表示カメラのTransformコンポーネントを参照する変数
        /// </summary>
        public Transform weaponCameraTransform;

        /// <summary>
        /// アイテムに関するクラスを参照する変数
        /// </summary>
        private ItemManager item;
        /// <summary>
        /// 棚の状態を管理するクラスを参照する変数
        /// </summary>
        private LockedShelf lockedShelf;
        /// <summary>
        /// ハンドガンの操作を管理するクラスを参照する変数
        /// </summary>
        private HandgunController handgunController;
        /// <summary>
        /// 鍵付きドアの状態を管理するクラスを参照する変数
        /// </summary>
        private LockedDoor lockedDoor;
        /// <summary>
        /// プレイヤーコントローラーのインスタンスを参照する変数
        /// </summary>
        public static PlayerController instance { get; private set; }

        /// <summary>
        /// プレイヤーのキャラクターコントローラーを参照する変数
        /// </summary>
        private CharacterController characterController;

        /// <summary>
        /// 視点移動の入力を参照する変数
        /// </summary>
        private Vector2 lookInput = Vector2.zero;
        /// <summary>
        /// 移動入力ベクトルを参照する変数
        /// </summary>
        private Vector2 moveInput = Vector2.zero;

        /// <summary>
        /// Rayが当たった情報を格納する変数
        /// </summary>
        private RaycastHit hit;

        /// <summary>
        /// Rayを参照する変数
        /// </summary>
        private Ray ray;

        /// <summary>
        /// ターゲットとなるインタラクト可能なオブジェクトを保持するための変数
        /// </summary>
        private I_Interactable currentTarget;
        /// <summary>
        /// Rayが当たったオブジェクトを参照する変数
        /// </summary>
        private I_Interactable interactable;

        /// <summary>
        /// レイヤーマスクを使用して、インタラクト可能なオブジェクトを特定するための変数
        /// </summary>
        public LayerMask interactableLayer = default;

        /// <summary>
        /// 首の前後移動の入力を保持するための変数
        /// </summary>
        private float translationZ = 0f;
        /// <summary>
        /// 回転角度を保持するための変数
        /// </summary>
        private float rotationX = 0f;
        /// <summary>
        /// 移動の速度を保持する速度を参照する変数
        /// </summary>
        private float currentSpeed;
        /// <summary>
        /// カメラのレイのX方向を参照する変数
        /// </summary>
        private float viewAngleX = 0.5f;
        /// <summary>
        /// カメラのレイのY方向を参照する変数
        /// </summary>
        private float viewAngleY = 0.5f;
        /// <summary>
        /// 移動入力の閾値を参照する変数
        /// </summary>
        private float moveInputThreshold = 0.01f;
        /// <summary>
        /// 重力の値を参照する変数
        /// </summary>
        private float gravityValue = -9.81f;
        /// <summary>
        /// 重力が地面にいるときに適用される値を参照する変数
        /// </summary>
        private float gravityCorrentValue = -1;
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
        /// 壁の当たり判定を行うための距離を参照する変数
        /// </summary>
        public float wallCheckerDistance = 0;
        /// <summary>
        /// 移動速度を参照する変数
        /// </summary>
        public float moveSpeed = 5.0f;

        /// <summary>
        /// プレイヤーの操作を有効にするかどうかを示すフラグを参照する変数
        /// </summary>
        public bool isSleeping = false;

        /// <summary>
        /// 棚のオブジェクトの名前を参照する変数
        /// </summary>
        private string shelfName = "StandObjectBedroom";
        /// <summary>
        /// ハンドガンのオブジェクトの名前を参照する変数
        /// </summary>
        private string handgunName = "HandgunRoot";
        /// <summary>
        /// ロックされたドアのオブジェクトの名前を参照する変数
        /// </summary>
        private string lockedDoorName = "Door03_pr";

        // モーション状態定義の列挙型
        private enum MotionState
        {
            Stopping,
            Walking
        }
        MotionState motionState = MotionState.Stopping;// 現在のモーション状態

        /// <summary>
        /// ゲーム開始時の初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            instance = this;

            // コンポーネントの登録
            characterController = GetComponent<CharacterController>();
            lockedShelf = GameObject.Find(shelfName).GetComponent<LockedShelf>();// 棚の状態を管理するクラスを検索して登録
            handgunController = GameObject.Find(handgunName).GetComponent<HandgunController>();// ハンドガンの操作を管理するクラスを検索して登録
            lockedDoor = GameObject.Find(lockedDoorName).GetComponent<LockedDoor>();// 鍵付きドアの状態を管理するクラスを検索して登録

            // カーソル設定
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;// カーソルを非表示にする

            // もしプレイヤー視点カメラが正しく設定されている場合
            if (mainCamera != null && mainCameraTransform != null)
            {
                // プレイヤー視点カメラをプレイヤー視点カメラ用ポジションの子にする
                mainCamera.transform.SetParent(mainCameraTransform);// プレイヤー視点カメラをプレイヤー視点カメラ用ポジションの子にする
                mainCamera.transform.localPosition = Vector3.zero;// プレイヤー視点カメラのローカル座標をゼロに設定
                mainCamera.transform.localRotation = Quaternion.identity;// プレイヤー視点カメラのローカル回転をゼロに設定
            }

            // もしアイテム表示カメラが正しく設定されている場合
            if (weaponCamera != null && weaponCameraTransform != null)
            {
                // アイテムカメラをアイテムカメラ用ポジションの子にする
                weaponCamera.transform.SetParent(weaponCameraTransform);// アイテム表示カメラをアイテム表示カメラ用ポジションの子にする
                weaponCamera.transform.localPosition = Vector3.zero;// アイテム表示カメラのローカル座標をゼロに設定
                weaponCamera.transform.localRotation = Quaternion.identity;// アイテム表示カメラのローカル回転をゼロに設定
            }
        }

        /// <summary>
        /// 毎フレーム呼び出される関数
        /// </summary>
        private void Update()
        {
            ray = mainCamera.ViewportPointToRay(new Vector3(viewAngleX, viewAngleY, 0f));// 画面の中心から奥へ向かうRayを作成

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

            UpdateMotionState(); // 状態を更新
            ApplyMovement();     // 移動を実行

            DetermineCurrentState();// 入力状況から最新の MotionState を決定する
        }

        /// <summary>
        /// 視点移動の入力を処理する関数
        /// </summary>
        /// <param name="context"></param>
        public void OnLook(InputAction.CallbackContext context)
        {
            if (!isSleeping)
            {
                lookInput = context.ReadValue<Vector2>();// 視点移動の入力を取得
            }
        }

        /// <summary>
        /// アイテムを拾うための入力を処理する関数
        /// </summary>
        /// <param name="context"></param>
        public void OnInteract(InputAction.CallbackContext context)
        {
            // もしインタラクトの入力が開始された場合
            if (context.performed && !isSleeping)
            {
                // もしターゲットが存在する場合
                if (currentTarget != null)
                {
                    PerformPickupInteraction();// アイテムを拾う準備を行い、拾う
                    return;
                }

                // もしクラスが棚の状態を管理するクラスを正しく参照している場合
                if (lockedShelf != null)
                {
                    lockedShelf.OpenShelf();// 棚を開ける処理
                    return;
                }

                // もしクラスがハンドガンの操作を管理するクラスを正しく参照している場合
                if (lockedDoor != null)
                {
                    lockedDoor.Interact();// ドアを開ける処理
                    return;
                }
            }
        }

        // Move アクションによって呼び出されるプログラムを移植（中山が編集）
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!isSleeping)
            {
                moveInput = context.ReadValue<Vector2>();
            }
        }

        /// <summary>
        /// インタラクト可能なオブジェクトを検出する関数
        /// </summary>
        private void CheckForInteractable()
        {
            // もしRayがインタラクト可能なオブジェクトに当たった場合
            if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
            {
                interactable = hit.collider.GetComponent<I_Interactable>();// Rayが当たったオブジェクトにI_Interactableコンポーネントがあるか確認するため登録

                // もしIInteractableコンポーネントがある場合
                if (interactable != null)
                {
                    currentTarget = interactable;// ターゲットを更新
                    return;
                }
            }

            currentTarget = null;
        }

        /// <summary>
        /// アイテムを拾う演出の準備を行う関数
        /// </summary>
        private void PerformPickupInteraction()
        {
            ray = mainCamera.ViewportPointToRay(new Vector3(viewAngleX, viewAngleY, 0f));// 画面の中心から奥へ向かうRayを作成

            // もしRayがオブジェクトに当たった場合
            if (Physics.Raycast(ray, out hit, interactRange))
            {
                item = hit.collider.GetComponent<ItemManager>(); // ヒットしたオブジェクトにItemManagerがついているか確認するため登録

                // もしItemManagerコンポーネントがある場合
                if (item != null)
                {
                    item.Pickup(mainCamera.transform, holdPosition);// アイテム側のPickup関数を呼び出す
                }
            }
        }

        /// <summary>
        /// モーション状態を更新する関数
        /// </summary>
        private void UpdateMotionState()
        {
            switch (motionState)
            {
                case MotionState.Stopping:
                    currentSpeed = 0f;
                    break;

                case MotionState.Walking:
                    currentSpeed = moveSpeed;

                    // ここに「歩き中のカメラの揺れ」などを追加できる

                    break;

                default:
                    currentSpeed = 0f;
                    break;
            }
        }

        /// <summary>
        /// 移動を実行する関数
        /// </summary>
        private void ApplyMovement()
        {
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;// カメラの向きに基づいた移動方向の計算

            //接地していない場合は下に引っ張る
            float gravity = characterController.isGrounded ? gravityCorrentValue : gravityValue;// 重力を適用するための変数
            Vector3 finalVelocity = move * currentSpeed;// 移動ベクトルを計算
            finalVelocity.y = gravity;// 重力を移動ベクトルに追加

            characterController.Move(finalVelocity * Time.deltaTime);// キャラクターコントローラーを使用して移動を実行
        }

        /// <summary>
        /// モーション状態を入力状況から決定する関数
        /// </summary>
        private void DetermineCurrentState()
        {
            // もし入力がない場合
            if (moveInput.sqrMagnitude < moveInputThreshold)
            {
                motionState = MotionState.Stopping;
            }
            else
            {
                motionState = MotionState.Walking;
            }
        }
    }
}
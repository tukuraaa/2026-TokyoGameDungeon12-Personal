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
        [SerializeField]
        private Camera _mainCamera;
        /// <summary>
        /// アイテム表示カメラの参照用変数
        /// </summary>
        [SerializeField]
        private Camera _weaponCamera;

        /// <summary>
        /// 首部分のTransformコンポーネントへの参照用変数
        /// </summary>
        [SerializeField]
        private Transform _neckTransform;
        /// <summary>
        /// カメラ右手元の位置を参照する変数
        /// </summary>
        [SerializeField]
        private Transform _holdPosition;
        /// <summary>
        /// プレイヤー視点カメラのPlayer内Transformコンポーネントを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _mainCameraPlayerTransform;
        /// <summary>
        /// アイテム表示カメラのPlayer内Transformコンポーネントを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _weaponCameraPlayerTransform;
        /// <summary>
        /// インタラクト操作のUIアイコンを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _interactControl_Icon;

        /// <summary>
        /// プレイヤー操作のUIを管理するクラスを参照する変数
        /// </summary>
        [SerializeField]
        private PlayerControllerUI_Manager _playerControllerUI_Manager;
        /// <summary>
        /// プレイヤー操作のUIを管理するクラスを参照する変数
        /// </summary>
        [SerializeField]
        private StageManager _stageManager;

        /// <summary>
        /// マウス感度を調整するための変数
        /// </summary>
        [SerializeField]
        private float _mouseSensitivity = 2.0f;
        /// <summary>
        /// インタラクト可能なオブジェクトを検出するための範囲を設定する変数
        /// </summary>
        [SerializeField]
        private float _interactRange = 2.5f;
        /// <summary>
        /// 移動速度を参照する変数
        /// </summary>
        [SerializeField]
        private float _moveSpeed = 5.0f;
        /// <summary>
        /// 待機時のカメラが揺れる速さを参照する変数
        /// </summary>
        [SerializeField]
        private float _idleBobSpeed = 1.0f;
        /// <summary>
        /// 待機時のカメラが揺れる揺れ幅（高さ）を参照する変数
        /// </summary>
        [SerializeField]
        private float _idleBobAmount = 0.02f;
        /// <summary>
        /// 歩行時のカメラが揺れる速さを参照する変数
        /// </summary>
        [SerializeField]
        private float _walkBobSpeed = 3.0f;
        /// <summary>
        /// 歩行時のカメラが揺れる揺れ幅（高さ）を参照する変数
        /// </summary>
        [SerializeField]
        private float _walkBobAmount = 0.08f;
        /// <summary>
        /// 状態が切り替わるときの滑らかさを参照する変数
        /// </summary>
        [SerializeField]
        private float _transitionSpeed = 5.0f;

        /// <summary>
        /// プレイヤーコントローラーのインスタンスを参照する変数
        /// </summary>
        public static PlayerController Instance { get; private set; }

        /// <summary>
        /// プレイヤーのキャラクターコントローラーを参照する変数
        /// </summary>
        private CharacterController _characterController;

        /// <summary>
        /// アイテムに関するクラスを参照する変数
        /// </summary>
        private ItemManager _itemManager;

        /// <summary>
        /// 視点移動の入力を参照する変数
        /// </summary>
        public Vector2 Look_Input = Vector2.zero;
        /// <summary>
        /// 移動入力ベクトルを参照する変数
        /// </summary>
        public Vector2 Move_Input = Vector2.zero;

        /// <summary>
        /// Rayが当たった情報を格納する変数
        /// </summary>
        private RaycastHit _hit;

        /// <summary>
        /// Rayを参照する変数
        /// </summary>
        private Ray _ray;

        /// <summary>
        /// ターゲットとなるインタラクト可能なオブジェクト（アイテム）を参照するための変数
        /// </summary>
        private I_Interactable _iCurrentTarget;
        /// <summary>
        /// Rayが当たったオブジェクト（アイテム）を参照する変数
        /// </summary>
        private I_Interactable _iInteractable;
        /// <summary>
        /// ターゲットとなるインタラクト可能なオブジェクト（棚）を参照するための変数
        /// </summary>
        private S_Interactable _sCurrentTarget;
        /// <summary>
        /// Rayが当たったオブジェクト（棚）を参照する変数
        /// </summary>
        private S_Interactable _sInteractable;
        /// <summary>
        /// ターゲットとなるインタラクト可能なオブジェクト（ドア）を参照するための変数
        /// </summary>
        private D_Interactable _dCurrentTarget;
        /// <summary>
        /// Rayが当たったオブジェクト（ドア）を参照する変数
        /// </summary>
        private D_Interactable _dInteractable;

        /// <summary>
        /// レイヤーマスクを使用して、インタラクト可能なオブジェクトを特定するための変数
        /// </summary>
        public LayerMask InteractableLayer;

        /// <summary>
        /// 首の前後移動の入力を保持するための変数
        /// </summary>
        private float _translationZ = 0f;
        /// <summary>
        /// 回転角度を保持するための変数
        /// </summary>
        private float _rotationX = 0f;
        /// <summary>
        /// 移動の速度を保持する速度を参照する変数
        /// </summary>
        private float _currentSpeed;
        /// <summary>
        /// カメラのレイのX方向を参照する変数
        /// </summary>
        private float _viewAngleX = 0.5f;
        /// <summary>
        /// カメラのレイのY方向を参照する変数
        /// </summary>
        private float _viewAngleY = 0.5f;
        /// <summary>
        /// 移動入力の閾値を参照する変数
        /// </summary>
        private float _moveInputThreshold = 0.01f;
        /// <summary>
        /// 重力の値を参照する変数
        /// </summary>
        private float _gravityValue = -9.81f;
        /// <summary>
        /// 重力が地面にいるときに適用される値を参照する変数
        /// </summary>
        private float _gravityCorrentValue = -1;
        /// <summary>
        /// 首の上下回転の制限角度を設定するための変数
        /// </summary>
        private float _minVertical = -90.0f;
        /// <summary>
        /// 最大の首の上下回転角度を設定するための変数
        /// </summary>
        private float _maxVertical = 90.0f;
        /// <summary>
        /// 首の前後移動の最小値を設定するための変数
        /// </summary>
        private float _minNeckTranslationZ = -1.0f;
        /// <summary>
        /// 首の前後移動の最大値を設定するための変数
        /// </summary>
        private float _maxNeckTranslationZ = 1.0f;
        /// <summary>
        /// 首の前後移動の感度を調整するための変数
        /// </summary>
        private float _adjustmentDivisor = 200.0f;
        /// <summary>
        /// タイマーを参照する変数
        /// </summary>
        private float _timer = 0.0f;
        /// <summary>
        /// カメラ揺れのY軸基準値を参照する変数
        /// </summary>
        private float _defaultY = 0.0f;
        /// <summary>
        /// 現在のカメラが揺れる速さを参照する変数
        /// </summary>
        private float _currentBobSpeed;
        /// <summary>
        /// 現在のカメラが揺れる揺れ幅を参照する変数
        /// </summary>
        private float _currentBobAmount;

        /// <summary>
        /// プレイヤーの操作を有効にするかどうかを示すフラグを参照する変数
        /// </summary>
        public bool IsSleeping = false;
        /// <summary>
        /// プレイヤーが歩いているかどうかの判定フラグを参照する変数
        /// </summary>
        private bool _isWalking = false;

        /// <summary>
        /// ターゲットとなるインタラクト可能オブジェクト種（棚）の名前を参照する変数
        /// </summary>
        private string _targetShelfName = "Shelf";
        /// <summary>
        /// ターゲットとなるインタラクト可能オブジェクト種（アイテム）の名前を参照する変数
        /// </summary>
        private string _targetItemName = "Item";
        /// <summary>
        /// ターゲットとなるインタラクト可能オブジェクト種（ドア）の名前を参照する変数
        /// </summary>
        private string _targetDoorName = "Door";

        /// <summary>
        /// モーション状態定義の列挙型
        /// </summary>
        private enum MotionState
        {
            Stopping,
            Walking
        }
        /// <summary>
        /// 現在のモーション状態を参照する変数
        /// </summary>
        private MotionState _motionState = MotionState.Stopping;

        /// <summary>
        /// ゲーム開始時の初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // もしインスタンスが無い場合
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // コライダーコンポーネントを登録
            _characterController = GetComponent<CharacterController>();

            // もしプレイヤー視点カメラとその座標が正しく設定されている場合
            if (_mainCamera != null && _mainCameraPlayerTransform != null)
            {
                // --- プレイヤー視点カメラをプレイヤーオブジェクト下に設定する ---
                // プレイヤー視点カメラをプレイヤー視点カメラ用ポジションの子にする
                _mainCamera.transform.SetParent(_mainCameraPlayerTransform);
                // プレイヤー視点カメラのローカル座標をゼロに設定
                _mainCamera.transform.localPosition = Vector3.zero;
                // プレイヤー視点カメラのローカル回転をゼロに設定
                _mainCamera.transform.localRotation = Quaternion.identity;
            }

            // もしアイテム表示カメラとその座標が正しく設定されている場合
            if (_weaponCamera != null && _weaponCameraPlayerTransform != null)
            {
                // --- アイテムカメラをプレイヤーオブジェクト下に設定する ---
                // アイテム表示カメラをアイテム表示カメラ用ポジションの子にする
                _weaponCamera.transform.SetParent(_weaponCameraPlayerTransform);
                // アイテム表示カメラのローカル座標をゼロに設定
                _weaponCamera.transform.localPosition = Vector3.zero;
                // アイテム表示カメラのローカル回転をゼロに設定
                _weaponCamera.transform.localRotation = Quaternion.identity;
            }

            // 開始時のカメラのY座標を基準値として保存
            _defaultY = _mainCamera.transform.localPosition.y;

            // --- 初期状態は待機時の値にする ---
            _currentBobSpeed = _idleBobSpeed;
            _currentBobAmount = _idleBobAmount;
        }

        /// <summary>
        /// 毎フレーム呼び出される関数
        /// </summary>
        private void Update()
        {
            // --- カメラ関係の管理関数を呼び出し ---
            // カメラ操作の管理を毎フレーム行う
            CameraControlManager();
            // カメラの揺れを毎フレーム行う（リアル演出用）
            CameraBobManager();

            // インタラクト可能なオブジェクトをチェック
            CheckInteractableManager();

            // 移動実行を管理する関数を呼び出す
            ApplyMovement();

            // --- MotionState管理関数を呼び出す ---
            // モーション状態の管理関数を呼び出し
            UpdateMotionState();
            // 入力状況から最新のMotionStateを決定する
            DetermineCurrentState();
        }

        /// <summary>
        /// 視点移動の入力を処理する関数
        /// </summary>
        /// <param name="_context"></param>
        public void OnLook(InputAction.CallbackContext _context)
        {
            // もしプレイヤーが動ける場合
            if (!IsSleeping)
            {
                // 視点移動の入力を取得
                Look_Input = _context.ReadValue<Vector2>();
            }
        }

        /// <summary>
        /// アイテムを拾うための入力を処理する関数
        /// </summary>
        /// <param name="_context"></param>
        public void OnInteract(InputAction.CallbackContext _context)
        {
            // もしプレイヤーが動けるかつ、インタラクトの入力が開始された場合
            if (_context.performed && !IsSleeping)
            {
                // もしターゲット（アイテム）が存在する場合
                if (_iCurrentTarget != null)
                {
                    // アイテムを拾う準備を行って拾う
                    PerformPickupInteraction();
                    // インタラクトUIを非表示にする
                    _playerControllerUI_Manager.TargetHide(_interactControl_Icon);

                    return;
                }
                // もしターゲット（棚）が存在する場合
                else if (_sCurrentTarget != null)
                {
                    // 棚を開ける処理
                    _sCurrentTarget.OpenShelf();
                    // インタラクトUIを非表示にする
                    _playerControllerUI_Manager.TargetHide(_interactControl_Icon);

                    return;
                }
                // もしターゲット（ドア）が存在する場合
                else if (_dCurrentTarget != null)
                {
                    // ドアを開ける処理
                    _dCurrentTarget.Interact();
                    // インタラクトUIを非表示にする
                    _playerControllerUI_Manager.TargetHide(_interactControl_Icon);

                    return;
                }
            }
        }

        /// <summary>
        /// 移動の入力を受け付ける関数
        /// </summary>
        /// <param name="_context"></param>
        public void OnMove(InputAction.CallbackContext _context)
        {
            // もしプレイヤーが動ける場合
            if (!IsSleeping)
            {

                Move_Input = _context.ReadValue<Vector2>();
            }
        }

        /// <summary>
        /// ポーズの入力を処理する関数
        /// </summary>
        /// <param name="_context"></param>
        public void OnPause(InputAction.CallbackContext _context)
        {
            // もしポーズの入力が開始された場合
            if (_context.performed)
            {
                _stageManager.Pause();
            }
        }

        /// <summary>
        /// インタラクト可能なオブジェクトを検出する関数
        /// </summary>
        private void CheckForI_Interactable(string _targetName)
        {
            // もしRayがインタラクト可能なオブジェクトに当たった場合
            if (Physics.Raycast(_ray, out _hit, _interactRange, InteractableLayer))
            {
                // もしターゲットの名前がアイテムと一致する場合
                if (_targetName == _targetItemName)
                {
                    // Rayが当たったオブジェクトにI_Interactableコンポーネントがあるか確認するため登録
                    _iInteractable = _hit.collider.GetComponent<I_Interactable>();
                }
                // もしターゲットの名前が棚と一致する場合
                else if (_targetName == _targetShelfName)
                {
                    // Rayが当たったオブジェクトの親オブジェクトにS_Interactableコンポーネントがあるか確認するため登録
                    _sInteractable = _hit.collider.GetComponentInParent<S_Interactable>();
                }
                // もしターゲットの名前がドアと一致する場合
                else if (_targetName == _targetDoorName)
                {
                    // Rayが当たったオブジェクトの親オブジェクトにD_Interactableコンポーネントがあるか確認するため登録
                    _dInteractable = _hit.collider.GetComponentInParent<D_Interactable>();
                }

                // もしIInteractableコンポーネントがある場合
                if (_iInteractable != null)
                {
                    // ターゲットを更新
                    _iCurrentTarget = _iInteractable;
                    // インタラクトUIを表示
                    _playerControllerUI_Manager.TargetShow(_interactControl_Icon);

                    return;
                }
                // もしSInteractableコンポーネントがある場合
                else if (_sInteractable != null)
                {
                    // ターゲットを更新
                    _sCurrentTarget = _sInteractable;
                    // インタラクトUIを表示
                    _playerControllerUI_Manager.TargetShow(_interactControl_Icon);

                    return;
                }
                // もしDInteractableコンポーネントがある場合
                else if (_dInteractable != null)
                {
                    // ターゲットを更新
                    _dCurrentTarget = _dInteractable;
                    // インタラクトUIを表示
                    _playerControllerUI_Manager.TargetShow(_interactControl_Icon);

                    return;
                }
            }

            // --- ターゲットをリセット ---
            _iCurrentTarget = null;
            _sCurrentTarget = null;
            _dCurrentTarget = null;

            // インタラクトUIを非表示にする
            _playerControllerUI_Manager.TargetHide(_interactControl_Icon);
        }

        /// <summary>
        /// アイテムを拾う演出の準備を行う関数
        /// </summary>
        private void PerformPickupInteraction()
        {
            // 画面の中心から奥へ向かうRayを作成
            _ray = _mainCamera.ViewportPointToRay(new Vector3(_viewAngleX, _viewAngleY, 0f));

            // もしRayがオブジェクトに当たった場合
            if (Physics.Raycast(_ray, out _hit, _interactRange))
            {
                // ヒットしたオブジェクトにItemManagerがついているか確認するため登録
                _itemManager = _hit.collider.GetComponent<ItemManager>();

                // もしItemManagerコンポーネントがある場合
                if (_itemManager != null)
                {
                    // アイテム側のPickup関数を呼び出す
                    _itemManager.Pickup(_mainCamera.transform, _holdPosition);
                }
            }
        }

        /// <summary>
        /// モーション状態を更新する関数
        /// </summary>
        private void UpdateMotionState()
        {
            // 動きの状態ごとに処理を分岐
            switch (_motionState)
            {
                case MotionState.Stopping:

                    // 現在の移動スピードを0にする
                    _currentSpeed = 0f;
                    // 歩いているかのフラグをオフ
                    _isWalking = false;

                    break;

                case MotionState.Walking:

                    // 現在の移動スピードに既定の移動スピードを代入
                    _currentSpeed = _moveSpeed;
                    // 歩いているかのフラグをオン
                    _isWalking = true;

                    break;

                default:

                    // 現在の移動スピードを0にする
                    _currentSpeed = 0f;
                    // 歩いているかのフラグをオフ
                    _isWalking = false;

                    break;
            }
        }

        /// <summary>
        /// 移動を実行する関数
        /// </summary>
        private void ApplyMovement()
        {
            // カメラの向きに基づいた移動方向の計算し参照する変数を定義
            Vector3 _moveVector = transform.right * Move_Input.x + transform.forward * Move_Input.y;

            // --- 接地していない場合は下に引っ張る ---
            // 重力を適用し参照する変数を定義
            float _gravity = _characterController.isGrounded ? _gravityCorrentValue : _gravityValue;
            // 移動ベクトルを計算し参照する変数を定義
            Vector3 _finalVelocity = _moveVector * _currentSpeed;
            // 重力を移動ベクトルに追加し参照する変数を定義
            _finalVelocity.y = _gravity;

            // キャラクターコントローラーを使用して移動を実行
            _characterController.Move(_finalVelocity * Time.deltaTime);
        }

        /// <summary>
        /// モーション状態を入力状況から決定する関数
        /// </summary>
        private void DetermineCurrentState()
        {
            // もし入力がない場合
            if (Move_Input.sqrMagnitude < _moveInputThreshold)
            {
                _motionState = MotionState.Stopping;
            }
            else
            {
                _motionState = MotionState.Walking;
            }
        }

        /// <summary>
        /// カメラ操作を管理する関数
        /// </summary>
        private void CameraControlManager()
        {
            // --- マウスの入力を感度とフレーム時間で調整して、回転と移動の値を更新 ---
            // マウスX方向の入力を感度とフレーム時間で調整し参照する変数を定義
            float _mouseRotationX = Look_Input.x * _mouseSensitivity * Time.deltaTime;
            // マウスY方向の入力を感度とフレーム時間で調整し参照する変数を定義
            float _mouseRotationY = Look_Input.y * _mouseSensitivity * Time.deltaTime;

            // マウスY方向の入力を感度とフレーム時間で調整して、首の前後移動の値を更新し参照する変数を定義
            float _mouseTranslationY = Look_Input.y * (_mouseSensitivity / _adjustmentDivisor) * Time.deltaTime;

            // 首の高さを保持するために現在の首のY位置を取得し参照する変数を定義
            float _neckTranslationY = _neckTransform.transform.localPosition.y;

            // プレイヤー（体）の左右の回転をマウスX方向の入力に合わせて行う
            transform.Rotate(0, _mouseRotationX, 0);

            // --- 首の回転と前後移動をマウスY方向の入力に合わせて更新 ---
            // マウスY方向の入力によって縦方向の回転を更新
            _rotationX -= _mouseRotationY;
            // 首の前後移動を指定された範囲に制限
            _translationZ = Mathf.Clamp(_translationZ, _minNeckTranslationZ, _maxNeckTranslationZ);
            // 首の回転を設定。縦方向のみ回転させる
            _neckTransform.localRotation = Quaternion.Euler(_rotationX, 0, 0);

            // --- 首の前後移動をマウスY方向の入力に合わせて更新 ---
            // マウスY方向の入力によって首の前後移動を更新
            _translationZ -= _mouseTranslationY;
            // 回転角度を指定された範囲に制限
            _rotationX = Mathf.Clamp(_rotationX, _minVertical, _maxVertical);
            // 首の位置を設定。前後移動のみ行う
            _neckTransform.localPosition = new Vector3(0, 0, _translationZ);

            // 首の高さを一定に保つ
            _neckTransform.localPosition = new Vector3(_neckTransform.localPosition.x, _neckTranslationY, _neckTransform.localPosition.z);
        }

        /// <summary>
        /// カメラの揺れを管理する関数
        /// </summary>
        private void CameraBobManager()
        {
            // --- 目標となる揺れの「速さ」と「幅」を決定し参照する変数を定義 ---
            float targetSpeed = _isWalking ? _walkBobSpeed : _idleBobSpeed;
            float targetAmount = _isWalking ? _walkBobAmount : _idleBobAmount;

            // --- 現在の値を目標値に向かって滑らかに変化させる（カクつき防止） ---
            _currentBobSpeed = Mathf.Lerp(_currentBobSpeed, targetSpeed, Time.deltaTime * _transitionSpeed);
            _currentBobAmount = Mathf.Lerp(_currentBobAmount, targetAmount, Time.deltaTime * _transitionSpeed);

            // --- サイン波を使った揺れの計算 ---
            // タイマーを現在のスピードで進める
            _timer += Time.deltaTime * _currentBobSpeed;
            // サイン波（-1 ～ 1 の間を行き来する値）に揺れ幅を掛け、基準の高さに足し参照する変数を定義
            float newY = _defaultY + Mathf.Sin(_timer) * _currentBobAmount;
            // カメラのローカル座標に適用
            _mainCamera.transform.localPosition = new Vector3(_mainCamera.transform.localPosition.x, newY, _mainCamera.transform.localPosition.z);
        }

        /// <summary>
        /// インタラクト可能なものを管理する関数
        /// </summary>
        private void CheckInteractableManager()
        {
            // 画面の中心から奥へ向かうRayを作成
            _ray = _mainCamera.ViewportPointToRay(new Vector3(_viewAngleX, _viewAngleY, 0f));

            // --- インタラクト可能なオブジェクトを検出する ---
            // インタラクト可能なオブジェクト（アイテム）を検出する関数を呼び出す
            CheckForI_Interactable(_targetItemName);
            // インタラクト可能なオブジェクト（棚）を検出する関数を呼び出す
            CheckForI_Interactable(_targetShelfName);
            // インタラクト可能なオブジェクト（ドア）を検出する関数を呼び出す
            CheckForI_Interactable(_targetDoorName);
        }
    }
}
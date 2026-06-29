using UnityEngine;
using System.Collections;

namespace PESDISASTER
{
    /// <summary>
    /// アイテムを拾うためのクラス
    /// </summary>
    public class ItemManager : MonoBehaviour, Item_Interactable
    {
        /// <summary>
        /// アイテムの物理演算を参照する変数
        /// </summary>
        private Rigidbody _itemRigidbody;

        /// <summary>
        /// アイテムのコライダーを参照する変数
        /// </summary>
        private Collider _itemCollider;

        /// <summary>
        /// アイテムを持つ目標位置を参照する変数
        /// </summary>
        private Transform _targetHoldPosition;

        /// <summary>
        /// アイテムの名前を参照する変数
        /// </summary>
        public string ItemName = "ハンドガン";
        /// <summary>
        /// アイテムの名前を定数で保持
        /// </summary>
        private string _handgunName = "ハンドガン";
        /// <summary>
        /// アイテムの名前を定数で保持
        /// </summary>
        private string _bedKeyName = "寝室の鍵";
        /// <summary>
        /// 特定のレイヤーの名前を参照する変数
        /// </summary>
        private string _layerName = "Hold_Item";

        /// <summary>
        /// アイテムを拾ったか否かのフラグを参照する変数
        /// </summary>
        private bool _isPickedUp = false;

        /// <summary>
        /// イージング時に調整するための値を参照する変数
        /// </summary>
        private float _easingNumber = 1f;
        /// <summary>
        /// アイテムが手元に移動するまでの時間を参照する変数
        /// </summary>
        private float _moveDuration = 0.5f;

        /// <summary>
        /// レイヤー名をIDに変換して保持するためにIDを参照する変数
        /// </summary>
        private int _hold_ItemLayer = -1;
        /// <summary>
        /// レイヤーが存在しない場合のIDを参照する変数
        /// </summary>
        private int _null_LayerNumber = -1;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // --- コンポーネントの登録 ---
            _itemRigidbody = GetComponent<Rigidbody>();
            _itemCollider = GetComponent<Collider>();

            // 毎回文字列でレイヤーを探すと重いため、最初にID（int）に変換して保持
            _hold_ItemLayer = LayerMask.NameToLayer(_layerName);

            // もしレイヤーが存在しなければ
            if (_hold_ItemLayer == _null_LayerNumber)
            {
                // 自分を無効にする
                this.enabled = false;
            }
        }

        /// <summary>
        /// プレイヤーのRaycast等のイベントから呼ばれる関数
        /// </summary>
        /// <param name="cameraTransform">メインカメラのTransform</param>
        /// <param name="holdPosition">手元の目標位置のTransform</param>
        public void Pickup(string itemName)
        {
            // もしすでに拾っている・無効な場合
            if (_isPickedUp  || PlayerController.Instance.MainCamera.transform == null)
            {
                return;
            }

            // 拾ったフラグをオン
            _isPickedUp = true;

            // RigidBodyがついている場合
            if (_itemRigidbody != null)
            {
                // --- 物理演算を無効化する ---
                _itemRigidbody.isKinematic = true;
                _itemRigidbody.useGravity = false;
            }

            // コライダーがついている場合
            if (_itemCollider != null)
            {
                // コライダーを無効化する
                _itemCollider.enabled = false;
            }

            // オブジェクトのレイヤーを変更
            SetLayerRecursively(gameObject, _hold_ItemLayer);

            // もし拾ったアイテムがハンドガンの場合
            if (ItemName == _handgunName)
            {
                // 手元の目標位置を右手元の位置にする
                _targetHoldPosition = PlayerController.Instance.RightHoldPosition;

                // このItemManagerが付いているオブジェクトの親（HandgunContainer）を取得
                WeaponFollower follower = GetComponentInParent<WeaponFollower>();

                // もしWeaponFollowerがついている場合
                if (follower != null)
                {
                    // WeaponFollowerの関数を呼び出して、カメラの動きに追従させる
                    follower.StartFollowing(PlayerController.Instance.MainCamera.transform);
                }
            }

            // もし拾ったアイテムが寝室の鍵の場合
            if (ItemName == _bedKeyName)
            {
                // 手元の目標位置を左手元の位置にする
                _targetHoldPosition = PlayerController.Instance.LeftHoldPosition;
            }

            // 手元の位置へ滑らかに移動させるコルーチンを開始
            StartCoroutine(MoveToHoldPosition(PlayerController.Instance.MainCamera.transform, _targetHoldPosition));

            // もし拾ったアイテムがハンドガンの場合
            if (ItemName == _handgunName)
            {
                // ハンドガンのチュートリアルを開始する
                PlayerController.Instance.PlayerControllerUI_ManagerClass.StartGunTutorial();
                // ハンドガンを装備する
                HandgunController.Instance.EquipGun(PlayerController.Instance.MainCamera);
            }
        }

        /// <summary>
        /// 子オブジェクトを含めて再帰的にレイヤーを変更する関数
        /// </summary>
        private void SetLayerRecursively(GameObject targetObject, int newLayer)
        {
            // 指定オブジェクトのレイヤーに新しくレイヤーを生成
            targetObject.layer = newLayer;

            // 全ての子オブジェクトを参照
            foreach (Transform child in targetObject.transform)
            {
                // レイヤーを変更
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        /// <summary>
        /// 手元の位置へ滑らかに移動させるコルーチン
        /// </summary>
        /// <param name="targetHoldPosition"></param>
        /// <returns></returns>
        private IEnumerator MoveToHoldPosition(Transform cameraTransform, Transform targetHoldPosition)
        {
            // アイテムを拾う経過時間を参照する変数
            float elapsedTime = 0f;

            // --- 移動開始時の、カメラから見た相対的な位置と回転を記録 ---
            // カメラの座標系での位置を計算
            Vector3 startRelativePos = cameraTransform.InverseTransformPoint(transform.position);
            // カメラの座標系での回転を計算
            Quaternion startRelativeRot = Quaternion.Inverse(cameraTransform.rotation) * transform.rotation;

            // --- 目標とする相対位置 ---
            // カメラの座標系での位置を計算
            Vector3 targetRelativePos = cameraTransform.InverseTransformPoint(targetHoldPosition.position);
            // カメラの座標系での回転を計算
            Quaternion targetRelativeRot = Quaternion.Inverse(cameraTransform.rotation) * targetHoldPosition.rotation;

            // アイテムが手元に移動するまでの時間がアイテムを拾う経過時間より長い間はループ
            while (elapsedTime < _moveDuration)
            {
                // 経過時間を0から1の範囲に正規化して、イージングする
                float time = Mathf.SmoothStep(0f, _easingNumber, elapsedTime / _moveDuration);

                // --- カメラの現在の位置・回転をベースに、補間した相対位置をワールド座標に変換して適用 ---
                // Lerpで滑らかに補間
                Vector3 currentRelativePos = Vector3.Lerp(startRelativePos, targetRelativePos, time);
                // Lerpで滑らかに補間
                Quaternion currentRelativeRot = Quaternion.Lerp(startRelativeRot, targetRelativeRot, time);

                // --- カメラの座標系での位置と回転をワールド座標に変換して適用 ---
                // カメラの座標系での位置をワールド座標に変換して適用
                transform.position = cameraTransform.TransformPoint(currentRelativePos);
                // カメラの回転に相対回転を掛けてワールド座標に変換して適用
                transform.rotation = cameraTransform.rotation * currentRelativeRot;

                // 経過時間を加算
                elapsedTime += Time.deltaTime;
                // 1フレーム待つ
                yield return null;
            }

            // LeftHoldPosition等と同じ親の子にする
            transform.SetParent(targetHoldPosition.parent);

            // --- 最後に目標の親のローカル座標にリセット ---
            // 目標の親のローカル位置にリセット
            transform.localPosition = targetHoldPosition.localPosition;
            // 目標の親のローカル回転にリセット
            transform.localRotation = targetHoldPosition.localRotation;
        }
    }
}
using UnityEngine;
using System.Collections;

namespace PESDISASTER
{
    /// <summary>
    /// アイテムを拾うためのクラス
    /// </summary>
    public class ItemManager : MonoBehaviour, I_Interactable
    {
        /// <summary>
        /// アイテムの物理演算を参照する変数
        /// </summary>
        private Rigidbody itemRigidbody;

        /// <summary>
        /// アイテムのコライダーを参照する変数
        /// </summary>
        private Collider itemCollider;

        /// <summary>
        /// カメラの位置を参照する変数
        /// </summary>
        private Transform cameraTransform;
        /// <summary>
        /// プレイヤー右手元の位置を参照する変数
        /// </summary>
        private Transform holdPosition;

        /// <summary>
        /// ハンドガンの機能を制御するクラスを参照する変数
        /// </summary>
        private HandgunController handgunController;

        /// <summary>
        /// カメラを参照する変数
        /// </summary>
        private Camera mainCamera;

        /// <summary>
        /// アイテムの名前を定数で保持
        /// </summary>
        private string handgunName = "ハンドガン";
        /// <summary>
        /// ハンドガンの管理オブジェクトの名前を参照する変数
        /// </summary>
        private string handgunRootName = "HandgunRoot";
        /// <summary>
        /// アイテムの名前を参照する変数
        /// </summary>
        public string itemName = "ハンドガン";
        /// <summary>
        /// カメラの名前を参照する変数
        /// </summary>
        private string cameraName = "MainCamera";
        /// <summary>
        /// 右手元の名前を参照する変数
        /// </summary>
        public string holdName = "RightHoldPosition";
        /// <summary>
        /// 特定のレイヤーの名前を参照する変数
        /// </summary>
        private string layerName = "Hold_Item";

        /// <summary>
        /// アイテムを拾ったか否かのフラグを参照する変数
        /// </summary>
        private bool isPickedUp = false;

        /// <summary>
        /// イージング時に調整するための値を参照する変数
        /// </summary>
        private float easingNumber = 1f;
        /// <summary>
        /// アイテムが手元に移動するまでの時間を参照する変数
        /// </summary>
        public float moveDuration = 0.5f;

        /// <summary>
        /// レイヤー名をIDに変換して保持するためにIDを参照する変数
        /// </summary>
        private int holdItemLayer = -1;
        /// <summary>
        /// レイヤーが存在しない場合のIDを参照する変数
        /// </summary>
        private int null_LayerNumber = -1;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // コンポーネントの登録
            itemRigidbody = GetComponent<Rigidbody>();
            itemCollider = GetComponent<Collider>();
            cameraTransform = GameObject.Find(cameraName).GetComponent<Transform>();
            holdPosition = GameObject.Find(holdName).GetComponent<Transform>();
            handgunController = GameObject.Find(handgunRootName).GetComponent<HandgunController>();
            mainCamera = GameObject.Find(cameraName).GetComponent<Camera>();


            holdItemLayer = LayerMask.NameToLayer(layerName);// 毎回文字列でレイヤーを探すと重いため、最初にID（int）に変換して保持

            // もしレイヤーが存在しなければ
            if (holdItemLayer == null_LayerNumber)
            {
                this.enabled = false;// 自分を無効にする
            }
        }

        /// <summary>
        /// 拾ったアイテムの名前を表示する関数
        /// </summary>
        /// <returns></returns>
        public string GetInteractText()
        {
            return $"{itemName} を拾う";// 拾ったアイテムの名前を表示する
        }

        /// <summary>
        /// プレイヤーのRaycast等のイベントから呼ばれる関数
        /// </summary>
        /// <param name="cameraTransform">メインカメラのTransform</param>
        /// <param name="holdPosition">手元の目標位置のTransform</param>
        public void Pickup(Transform cameraTransform, Transform holdPosition)
        {
            // もしすでに拾っている・無効な場合
            if (isPickedUp || holdPosition == null || cameraTransform == null)
            {
                return;
            }

            isPickedUp = true;// 拾ったフラグをオン

            // RigidBodyがついている場合
            if (itemRigidbody != null)
            {
                // 物理演算を無効化する
                itemRigidbody.isKinematic = true;
                itemRigidbody.useGravity = false;
            }

            // コライダーがついている場合
            if (itemCollider != null)
            {
                itemCollider.enabled = false;// コライダーを無効化する
            }

            SetLayerRecursively(gameObject, holdItemLayer);// オブジェクトのレイヤーを変更

            WeaponFollower follower = GetComponentInParent<WeaponFollower>();// このItemManagerが付いているオブジェクトの親（HandgunContainer）を取得

            // もしWeaponFollowerがついている場合
            if (follower != null)
            {
                follower.StartFollowing(cameraTransform);// WeaponFollowerの関数を呼び出して、カメラの動きに追従させる
            }

            StartCoroutine(MoveToHoldPosition(cameraTransform, holdPosition));// 手元の位置へ滑らかに移動させるコルーチンを開始

            // もし拾ったアイテムがハンドガンの場合
            if (itemName == handgunName)
            {
                handgunController.EquipGun(mainCamera);// ハンドガンを装備する
            }
        }

        /// <summary>
        /// 子オブジェクトを含めて再帰的にレイヤーを変更する関数
        /// </summary>
        private void SetLayerRecursively(GameObject obj, int newLayer)
        {
            obj.layer = newLayer;

            // 全ての子オブジェクトを参照
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, newLayer);// レイヤーを変更
            }
        }

        /// <summary>
        /// 手元の位置へ滑らかに移動させるコルーチン
        /// </summary>
        /// <param name="targetHoldPosition"></param>
        /// <returns></returns>
        private IEnumerator MoveToHoldPosition(Transform cameraTransform, Transform targetHoldPosition)
        {
            float elapsedTime = 0f;// アイテムを拾う経過時間を参照する変数

            // 移動開始時の、カメラから見た相対的な位置と回転を記録
            Vector3 startRelativePos = cameraTransform.InverseTransformPoint(transform.position);// カメラの座標系での位置を計算
            Quaternion startRelativeRot = Quaternion.Inverse(cameraTransform.rotation) * transform.rotation;// カメラの座標系での回転を計算

            // 目標とする相対位置
            Vector3 targetRelativePos = cameraTransform.InverseTransformPoint(targetHoldPosition.position);// カメラの座標系での位置を計算
            Quaternion targetRelativeRot = Quaternion.Inverse(cameraTransform.rotation) * targetHoldPosition.rotation;// カメラの座標系での回転を計算

            // アイテムが手元に移動するまでの時間がアイテムを拾う経過時間より長い間はループ
            while (elapsedTime < moveDuration)
            {
                float time = Mathf.SmoothStep(0f, easingNumber, elapsedTime / moveDuration);// 経過時間を0から1の範囲に正規化して、イージングする

                // カメラの現在の位置・回転をベースに、補間した相対位置をワールド座標に変換して適用
                Vector3 currentRelativePos = Vector3.Lerp(startRelativePos, targetRelativePos, time);// Lerpで滑らかに補間
                Quaternion currentRelativeRot = Quaternion.Lerp(startRelativeRot, targetRelativeRot, time);// Lerpで滑らかに補間

                // カメラの座標系での位置と回転をワールド座標に変換して適用
                transform.position = cameraTransform.TransformPoint(currentRelativePos);// カメラの座標系での位置をワールド座標に変換して適用
                transform.rotation = cameraTransform.rotation * currentRelativeRot;// カメラの回転に相対回転を掛けてワールド座標に変換して適用

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 最後に目標の親のローカル座標にリセット
            transform.localPosition = targetHoldPosition.localPosition;// 目標の親のローカル位置にリセット
            transform.localRotation = targetHoldPosition.localRotation;// 目標の親のローカル回転にリセット
        }
    }
}
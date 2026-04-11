using UnityEngine;

namespace PESDISASTER
{
    public class WeaponFollower : MonoBehaviour
    {
        /// <summary>
        /// 追従する対象を参照する変数
        /// </summary>
        private Transform targetCamera;

        /// <summary>
        /// 追従を開始するかどうかのフラグ
        /// </summary>
        private bool isFollowing = false;

        /// <summary>
        /// ItemManagerからこれを呼んで追従を開始させる関数
        /// </summary>
        /// <param name="cameraTransform"></param>
        public void StartFollowing(Transform cameraTransform)
        {
            targetCamera = cameraTransform;// 追従する対象を設定
            isFollowing = true;// 追従を開始するフラグをオン
        }

        /// <summary>
        /// 毎フレーム、LateUpdateでカメラの位置と回転を追従させる関数
        /// </summary>
        private void Update()
        {
            // もし追従が有効で、対象のカメラが存在する場合
            if (isFollowing && targetCamera != null)
            {
                transform.SetPositionAndRotation(targetCamera.position, targetCamera.rotation);// カメラと全く同じ位置・回転にする
            }
        }
    }
}
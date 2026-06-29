using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// 武器をカメラに追従させるクラス
    /// </summary>
    public class WeaponFollower : MonoBehaviour
    {
        /// <summary>
        /// 追従する対象を参照する変数
        /// </summary>
        private Transform _targetCamera;

        /// <summary>
        /// 追従を開始するかどうかのフラグ
        /// </summary>
        private bool _isFollowing = false;

        /// <summary>
        /// ItemManagerからこれを呼んで追従を開始させる関数
        /// </summary>
        /// <param name="cameraTransform"></param>
        public void StartFollowing(Transform cameraTransform)
        {
            // 追従する対象を設定
            _targetCamera = cameraTransform;
            // 追従を開始するフラグをオン
            _isFollowing = true;
        }

        /// <summary>
        /// 毎フレーム、LateUpdateでカメラの位置と回転を追従させる関数
        /// </summary>
        private void Update()
        {
            // もし追従が有効で、対象のカメラが存在する場合
            if (_isFollowing && _targetCamera != null)
            {
                // カメラと全く同じ位置・回転にする
                transform.SetPositionAndRotation(_targetCamera.position, _targetCamera.rotation);
            }
        }
    }
}
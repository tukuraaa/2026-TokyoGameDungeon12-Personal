using UnityEngine;

namespace PESDISASTER
{
    public class WeaponFollower : MonoBehaviour
    {
        /// <summary>
        /// 追従する対象を参照する変数
        /// </summary>
        public Transform targetCamera;

        /// <summary>
        /// 毎フレーム、LateUpdateでカメラの位置と回転を追従させる関数
        /// </summary>
        private void LateUpdate()
        {
            // もし追従する対象が設定されている場合
            if (targetCamera != null)
            {
                // 位置と回転をカメラと完全に一致させる
                transform.position = targetCamera.position;// カメラの位置に武器を移動させる
                transform.rotation = targetCamera.rotation;// カメラの回転に武器を回転させる
            }
        }
    }
}
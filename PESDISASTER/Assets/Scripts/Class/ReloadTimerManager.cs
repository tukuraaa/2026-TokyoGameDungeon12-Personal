using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// リロード時間を測定するタイマーを管理するクラス
    /// </summary>
    public class ReloadTimerManager : MonoBehaviour
    {
        private float m_fTimer;
        public float CurrentTime { get { return m_fTimer; } }

        public bool m_bActive = false;

        private void Update()
        {
            if (m_bActive)
            {
                m_fTimer += Time.deltaTime;
            }
        }

        public void OnStart()
        {
            m_bActive = true;
        }
        public void OnStop()
        {
            m_bActive = false;
        }
        public void OnReset()
        {
            m_fTimer = 0f;
            OnStop();
        }
    }
}
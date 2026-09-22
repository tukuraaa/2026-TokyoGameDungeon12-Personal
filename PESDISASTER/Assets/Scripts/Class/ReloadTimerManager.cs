using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// リロード時間を測定するタイマーを管理するクラス
    /// </summary>
    public class ReloadTimerManager : MonoBehaviour
    {
        /// <summary>
        /// シングルトンインスタンスを参照する変数
        /// </summary>
        public static ReloadTimerManager Instance { get; private set; }

        /// <summary>
        /// 現在の経過時間を参照する変数
        /// </summary>
        public float CurrentTime;

        /// <summary>
        /// タイマーが有効かどうか判別するフラグ変数
        /// </summary>
        private bool _timerActive = false;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 毎フレーム処理を行う関数
        /// </summary>
        private void Update()
        {
            if (_timerActive)
            {
                CurrentTime += Time.deltaTime;
            }
        }

        /// <summary>
        /// 時間測定を開始する関数
        /// </summary>
        public void StartTimer()
        {
            _timerActive = true;
        }

        /// <summary>
        /// 時間測定を一時停止する関数
        /// </summary>
        public void StopTimer()
        {
            _timerActive = false;
        }

        /// <summary>
        /// タイマーをリセットする関数
        /// </summary>
        public void ResetTimer()
        {
            CurrentTime = 0f;
            StopTimer();
        }
    }
}
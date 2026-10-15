using System.Collections;
using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// プレイヤー操作方法UIを管理するクラス
    /// </summary>
    public class PlayerControlUIManager : MonoBehaviour
    {
        /// <summary>
        /// 基礎操作方法UIを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _defaultControlTutorialUI;
        /// <summary>
        /// 銃操作方法UIを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _gunControlTutorialUI;
        /// <summary>
        /// リロードミニゲームのチュートリアルUIを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _reloadMinigameTutorialUI;

        /// <summary>
        /// 対象の透明バリア管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private BarrierManager _targetBarrierManager;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// シングルトンインスタンスを参照する変数
        /// </summary>
        public static PlayerControlUIManager Instance { get; private set; }

        /// <summary>
        /// 操作チュートリアルのイントロトリガーを参照する変数
        /// </summary>
        private static readonly int _tutorialIntroID = Animator.StringToHash("OnTutorialStart");
        /// <summary>
        /// 操作チュートリアルのアウトロトリガーを参照する変数
        /// </summary>
        private static readonly int _tutorialOutroID= Animator.StringToHash("OnTutorialEnd");

        /// <summary>
        /// チュートリアル演出時間を参照する変数
        /// </summary>
        private float _tutorialAnimTime = 10f;

        /// <summary>
        /// チュートリアル中かどうかを判別する変数
        /// </summary>
        private bool _isTutorial = false;

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

            _animator = GetComponent<Animator>();
            
            // 最初はUIの子オブジェクトを非表示
            UIGeneralManager.Instance.SetActiveChildUI(false,this.transform);
        }

        /// <summary>
        /// 操作チュートリアルのイントロ処理・演出を行うコルーチン 
        /// </summary>
        /// <param name="targetUI">対象のUIを参照する変数</param>
        /// <returns></returns>
        private IEnumerator ControlTutorialIntroCoroutine(Transform targetUI)
        {
            if (_isTutorial) { yield break; }

            _isTutorial = true;
            // 指定のUIを表示
            UIGeneralManager.Instance.SetActiveUI(true,targetUI);
            _animator.SetTrigger(_tutorialIntroID);
            yield return new WaitForSeconds(_tutorialAnimTime);
        }

        /// <summary>
        /// 操作チュートリアルのアウトロ処理・演出を行うコルーチン 
        /// </summary>
        /// <param name="targetUI">対象のUIを参照する変数</param>
        /// <returns></returns>
        private IEnumerator ControlTutorialOutroCoroutine(Transform targetUI)
        {
            _animator.SetTrigger(_tutorialOutroID);
            yield return new WaitForSeconds(_tutorialAnimTime);
            // 指定のUIを非表示
            UIGeneralManager.Instance.SetActiveUI(false, targetUI);
            _isTutorial = false;
        }

        /// <summary>
        /// プレイヤー基本操作チュートリアルを開始する関数
        /// </summary>
        public void StartDefaultTutorial()
        {
            // チュートリアル演出を呼び出し
            StartCoroutine(ControlTutorialIntroCoroutine(_defaultControlTutorialUI));
        }
    }
}
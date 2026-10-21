using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PESDISASTER
{
    /// <summary>
    /// プレイヤー操作方法UIを管理するクラス
    /// </summary>
    public class PlayerControlUIManager : MonoBehaviour
    {
        /// <summary>
        /// 対象の透明バリア管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private BarrierManager _targetBarrierManager;

        /// <summary>
        /// 戻るボタンを参照する変数
        /// </summary>
        [SerializeField]
        private Button _backButton;

        /// <summary>
        /// チュートリアル用のスクショ画像を参照する変数
        /// </summary>
        [SerializeField]
        private Image _tutorialScImage;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// シングルトンインスタンスを参照する変数
        /// </summary>
        public static PlayerControlUIManager Instance 
        {
            get;
            private set; 
        }

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
            if (Instance == null){Instance = this;}
            else{Destroy(gameObject);}

            _animator = GetComponent<Animator>();

            _backButton.onClick.AddListener(EndTutorial);
            
            // 最初はUIの子オブジェクトを非表示
            UIGeneralManager.Instance.SetActiveChildUI(false,this.transform);
        }

        /// <summary>
        /// 操作チュートリアルのイントロ処理・演出を行うコルーチン 
        /// </summary>
        /// <returns></returns>
        private IEnumerator ControlTutorialIntroCoroutine()
        {
            if (_isTutorial) { yield break; }

            _isTutorial = true;
            // UIを表示
            UIGeneralManager.Instance.SetActiveChildUI(true, this.transform);
            _animator.SetTrigger(_tutorialIntroID);
            yield return new WaitForSeconds(_tutorialAnimTime);
        }

        /// <summary>
        /// 操作チュートリアルのアウトロ処理・演出を行うコルーチン 
        /// </summary>
        /// <returns></returns>
        private IEnumerator ControlTutorialOutroCoroutine()
        {
            _animator.SetTrigger(_tutorialOutroID);
            yield return new WaitForSeconds(_tutorialAnimTime);
            // UIを非表示
            UIGeneralManager.Instance.SetActiveChildUI(false, this.transform);
            _isTutorial = false;
        }

        /// <summary>
        /// プレイヤー操作チュートリアルを開始する関数
        /// </summary>
        public void StartTutorial()
        {
            // チュートリアルイントロ演出を呼び出し
            StartCoroutine(ControlTutorialIntroCoroutine());
        }

        /// <summary>
        /// プレイヤー操作チュートリアルを終了する関数
        /// </summary>
        private void EndTutorial()
        {
            // チュートリアルアウトロ演出を呼び出し
            StartCoroutine(ControlTutorialOutroCoroutine());
        }
    }
}
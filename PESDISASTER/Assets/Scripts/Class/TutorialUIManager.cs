using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PESDISASTER
{
    /// <summary>
    /// プレイヤー操作方法UIを管理するクラス
    /// </summary>
    public class TutorialUIManager : MonoBehaviour
    {
        /// <summary>
        /// 指定のローカライズテキスト管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private LocalizedTextDisplay _targetLocalizedTextDisplay;

        /// <summary>
        /// 戻るボタンを参照する変数
        /// </summary>
        [SerializeField]
        private Button _backButton;

        /// <summary>
        /// チュートリアル用スクショ画像を参照する変数
        /// </summary>
        [SerializeField]
        private Image _tutorialScImage;

        /// <summary>
        /// チュートリアル用スクショ画像の素材そのものを参照する変数
        /// </summary>
        [SerializeField]
        private Sprite[] _tutorialScSprite;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// シングルトンインスタンスを参照する変数
        /// </summary>
        public static TutorialUIManager Instance
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
        private static readonly int _tutorialOutroID = Animator.StringToHash("OnTutorialEnd");

        /// <summary>
        /// チュートリアル演出時間を参照する変数
        /// </summary>
        private float _tutorialAnimTime = 1f;

        /// <summary>
        /// 基本操作チュートリアル用スクショ画像の素材IDを参照する変数
        /// </summary>
        private int _normalTutorialSpriteID = 0;
        /// <summary>
        /// 現在のチュートリアル用スクショ画像IDを参照する変数
        /// </summary>
        private int _currentTutorialSpriteID;

        /// <summary>
        /// チュートリアル中かどうかを判別する変数
        /// </summary>
        private bool _isTutorial = false;

        /// <summary>
        /// 基本チュートリアルのID名を参照する変数
        /// </summary>
        public string NormalTutorialIDName = "NormalControlTutorial";
        /// <summary>
        /// 現在のチュートリアルID名を参照する変数
        /// </summary>
        private string _currentTutorialIDName;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else
            {
                this.enabled = false;
                Destroy(gameObject);
                return;
            }

            _animator = GetComponent<Animator>();

            _backButton.onClick.AddListener(EndTutorial);

            // 最初は戻るボタンの機能をオフ
            _backButton.enabled = false;
            // 最初はUIの子オブジェクトを非表示
            UIGeneralManager.Instance.SetActiveChildUI(this.transform, false);
        }

        /// <summary>
        /// 操作チュートリアルのイントロ処理・演出を行うコルーチン 
        /// </summary>
        /// <param name="idName">個別のチュートリアルIDの名前を参照する変数</param>
        /// <returns></returns>
        private IEnumerator ControlTutorialIntroCoroutine(string idName)
        {
            if (_isTutorial) { yield break; }

            _isTutorial = true;

            // このチュートリアルが基本操作の場合
            if (idName == NormalTutorialIDName)
            {
                // 現在のチュートリアルIDを基本操作のIDに変更
                _currentTutorialIDName = NormalTutorialIDName;
                // 現在のチュートリアル画像素材IDを基本操作のIDに変更
                _currentTutorialSpriteID = _normalTutorialSpriteID;
            }

            // テキストを変更
            _targetLocalizedTextDisplay.TextKey = _currentTutorialIDName;
            // スクショ画像を変更
            _tutorialScImage.sprite = _tutorialScSprite[_currentTutorialSpriteID];

            PlayerController.Instance.IsCanPause = false;
            PlayerController.Instance.IsCanMove = false;
            UIGeneralManager.Instance.SetActiveChildUI(this.transform, true);
            _animator.SetTrigger(_tutorialIntroID);
            yield return new WaitForSeconds(_tutorialAnimTime);
            _backButton.enabled = true;

            // --- カーソルロック解除 --------------------------
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // ---------------------------------------------
        }

        /// <summary>
        /// 操作チュートリアルのアウトロ処理・演出を行うコルーチン 
        /// </summary>
        /// <returns></returns>
        private IEnumerator ControlTutorialOutroCoroutine()
        {
            // --- カーソルロック --------------------------
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // ---------------------------------------------

            _backButton.enabled = false;
            _animator.SetTrigger(_tutorialOutroID);
            yield return new WaitForSeconds(_tutorialAnimTime);
            // UIを非表示
            UIGeneralManager.Instance.SetActiveChildUI(this.transform, false);
            PlayerController.Instance.IsCanMove = true;
            PlayerController.Instance.IsCanPause = true;

            // このチュートリアルが基本操作の場合、チュートリアル終了後にエイムアイコンUI（酔い防止の白点）を表示
            if (_currentTutorialIDName == NormalTutorialIDName) { PlayerStatusUIManager.Instance.ShowStatusUI(PlayerStatusUIManager.Instance.AimUI); }

            _isTutorial = false;
        }

        /// <summary>
        /// プレイヤー操作チュートリアルを開始する関数
        /// </summary>
        /// <param name="idName">個別のチュートリアルIDの名前を参照する変数</param>
        public void StartTutorial(string idName)
        {
            // チュートリアルイントロ演出を呼び出し
            StartCoroutine(ControlTutorialIntroCoroutine(idName));
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
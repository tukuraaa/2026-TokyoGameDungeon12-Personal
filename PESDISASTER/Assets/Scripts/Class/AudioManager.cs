using System.Collections.Generic;
using UnityEngine;

namespace PESDISASTER
{
    /// <summary>
    /// BGMサウンド種類の列挙型
    /// </summary>
    public enum BGM_Type
    {
        Title,
        Intro,
        Stage1,
        GameOver,
        Clear
    }
    /// <summary>
    /// SEサウンド種類の列挙型
    /// </summary>
    public enum SE_Type
    {
        Cursor,
        Click,
        EmptyMagazine,
        Reload,
        Shoot,
        Notice,
        Pause,
        Scream,
        Paper
    }

    /// <summary>
    /// インスペクターでBGMを設定・管理するためのデータクラス
    /// </summary>
    [System.Serializable]
    public class BGM_Data
    {
        /// <summary>
        /// BGM素材そのものを参照する変数
        /// </summary>
        public AudioClip Clip;

        /// <summary>
        /// BGM種類を参照する変数
        /// </summary>
        public BGM_Type BGM_Type;

        /// <summary>
        /// BGM音量を参照する変数
        /// </summary>
        [Range(0f, 1f)]
        public float Volume = 1.0f;
    }
    /// <summary>
    /// インスペクターでSEを設定・管理するためのデータクラス
    /// </summary>
    [System.Serializable]
    public class SE_Data
    {
        /// <summary>
        /// SE素材そのものを参照する変数
        /// </summary>
        public AudioClip Clip;

        /// <summary>
        /// SE種類を参照する変数
        /// </summary>
        public SE_Type SE_Type;

        /// <summary>
        /// SE音量を参照する変数
        /// </summary>
        [Range(0f, 1f)]
        public float Volume = 1.0f;
    }

    /// <summary>
    /// 音響を管理するクラス
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// BGMデータを参照する変数の配列
        /// </summary>
        [SerializeField]
        private BGM_Data[] _bGM_DataArray;
        /// <summary>
        /// SEデータを参照する変数の配列
        /// </summary>
        [SerializeField]
        private SE_Data[] _sE_DataArray;

        /// <summary>
        /// 同時に鳴らせるSEの最大数
        /// </summary>
        [SerializeField]
        private int _maxSE_Channels = 10;

        /// <summary>
        /// どこからでもアクセスできるシングルトンインスタンスの変数
        /// </summary>
        public static AudioManager Instance { get; private set; }

        /// <summary>
        /// BGMの種類・データをまとめて参照するディクショナリ変数
        /// </summary>
        private Dictionary<BGM_Type, BGM_Data> _bGM_Dictionary = new Dictionary<BGM_Type, BGM_Data>();
        /// <summary>
        /// SEの種類・データをまとめて参照するディクショナリ変数
        /// </summary>
        private Dictionary<SE_Type, SE_Data> _sE_Dictionary = new Dictionary<SE_Type, SE_Data>();

        /// <summary>
        /// BGMのソースを参照する変数
        /// </summary>
        private AudioSource _bGM_Source;

        /// <summary>
        /// SE同時再生のためのAudioSourceプールを参照するリスト変数
        /// </summary>
        private List<AudioSource> _sE_SourceList = new List<AudioSource>();

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // もしシングルトンではない場合
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // 音響設定の初期化
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 音響設定の初期化を行う関数
        /// </summary>
        private void Initialize()
        {
            // --- BGMの初期化 ---
            _bGM_Source = gameObject.AddComponent<AudioSource>();
            _bGM_Source.loop = true;
            _bGM_Source.playOnAwake = false;

            // BGMデータをスキャン
            foreach (var bGM in _bGM_DataArray)
            {
                // もしディクショナリ変数が空の場合
                if (!_bGM_Dictionary.ContainsKey(bGM.BGM_Type))
                {
                    // ディクショナリ変数にデータを追加
                    _bGM_Dictionary.Add(bGM.BGM_Type, bGM);
                }
            }

            // 存在しているSEのチャンネル分ループ
            for (int i = 0; i < _maxSE_Channels; i++)
            {
                // --- SEの初期化（チャンネルの確保） ---
                AudioSource seSource = gameObject.AddComponent<AudioSource>();
                seSource.playOnAwake = false;

                // リストにSEソースを追加
                _sE_SourceList.Add(seSource);
            }

            // SEのデータをスキャン
            foreach (var sE in _sE_DataArray)
            {
                // もしディクショナリ変数が空の場合
                if (!_sE_Dictionary.ContainsKey(sE.SE_Type))
                {
                    // ディクショナリ変数にデータを追加
                    _sE_Dictionary.Add(sE.SE_Type, sE);
                }
            }
        }

        /// <summary>
        /// BGMを再生する関数
        /// </summary>
        public void PlayBGM(BGM_Type type)
        {
            // もしディクショナリ変数に中身がある場合
            if (_bGM_Dictionary.TryGetValue(type, out BGM_Data data))
            {
                // もし同じBGMが既に鳴っている場合
                if (_bGM_Source.clip == data.Clip && _bGM_Source.isPlaying)
                {
                    return;
                }

                // --- BGMソースを指定の設定にして再生 ---
                _bGM_Source.clip = data.Clip;
                _bGM_Source.volume = data.Volume;
                _bGM_Source.Play();
            }
        }

        /// <summary>
        /// BGMを停止する関数
        /// </summary>
        public void StopBGM() => _bGM_Source.Stop();

        /// <summary>
        /// SEを再生する関数
        /// </summary>
        public void PlaySE(SE_Type type)
        {
            // もしディクショナリ変数に中身がある場合
            if (_sE_Dictionary.TryGetValue(type, out SE_Data data))
            {
                // 空いているAudioSource（再生中でないもの）を探して鳴らす
                foreach (var source in _sE_SourceList)
                {
                    // もしプレイ中のソースが無い場合
                    if (!source.isPlaying)
                    {
                        // --- SEソースを指定の設定にして再生 ---
                        source.clip = data.Clip;
                        source.volume = data.Volume;
                        source.Play();

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// ボタンに触れた時の音を再生する際に使用する関数
        /// </summary>
        public void PlayCursorSE()
        {
            // 指定の音を再生
            PlaySE(SE_Type.Cursor);
        }

        /// <summary>
        /// ボタンを押した時の音を再生する際に使用する関数
        /// </summary>
        public void PlayClickSE()
        {
            // 指定の音を再生
            PlaySE(SE_Type.Click);
        }
    }
}
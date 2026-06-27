using System.Collections.Generic;
using UnityEngine;

namespace PESDISASTER
{

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
        public string BGM_Type;

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
        public string SE_Type;

        /// <summary>
        /// SE音量を参照する変数
        /// </summary>
        [Range(0f, 1f)]
        public float Volume = 1.0f;

        /// <summary>
        /// ループするかを判別するフラグを参照する変数
        /// </summary>
        public bool IsLooping = false;
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
        private Dictionary<string, BGM_Data> _bGM_Dictionary = new Dictionary<string, BGM_Data>();
        /// <summary>
        /// SEの種類・データをまとめて参照するディクショナリ変数
        /// </summary>
        private Dictionary<string, SE_Data> _sE_Dictionary = new Dictionary<string, SE_Data>();

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
        public void PlayBGM(string type)
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
        /// BGSを停止する関数
        /// </summary>
        public void StopBGS(string type)
        {
            // もしディクショナリ変数に中身がある場合
            if (_sE_Dictionary.TryGetValue(type, out SE_Data _data))
            {
                // 空いているAudioSource（再生中でないもの）を探して鳴らす
                foreach (var _source in _sE_SourceList)
                {
                    // もしプレイ中の場合
                    if (_source.isPlaying)
                    {
                        // --- BGSを停止する ---
                        _source.Stop();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// SEを再生する関数
        /// </summary>
        public void PlaySE(string type)
        {
            // もしディクショナリ変数に中身がある場合
            if (_sE_Dictionary.TryGetValue(type, out SE_Data _data))
            {
                // 空いているAudioSource（再生中でないもの）を探して鳴らす
                foreach (var _source in _sE_SourceList)
                {
                    // もしプレイ中のソースが無い場合
                    if (!_source.isPlaying)
                    {
                        // --- SEソースを指定の設定にして再生 ---
                        _source.clip = _data.Clip;
                        _source.volume = _data.Volume;

                        // もしループ再生したい場合
                        if (_data.IsLooping)
                        {
                            // ループ再生する
                            _source.loop = true;
                        }
                        else
                        {
                            // 一度だけ再生
                            _source.loop = false;
                        }

                        // --- SEを再生する ---
                        _source.Play();
                        return;
                    }
                }
            }
        }
    }
}
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
        Message,
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
        public BGM_Data[] bgmDataArray;
        /// <summary>
        /// SEデータを参照する変数の配列
        /// </summary>
        public SE_Data[] seDataArray;
        /// <summary>
        /// どこからでもアクセスできるシングルトンインスタンス
        /// </summary>
        public static AudioManager instance { get; private set; }

        /// <summary>
        /// BGMの種類・データをまとめて参照するディクショナリ変数
        /// </summary>
        private Dictionary<BGM_Type, BGM_Data> bgmDict = new Dictionary<BGM_Type, BGM_Data>();
        /// <summary>
        /// SEの種類・データをまとめて参照するディクショナリ変数
        /// </summary>
        private Dictionary<SE_Type, SE_Data> seDict = new Dictionary<SE_Type, SE_Data>();

        /// <summary>
        /// BGMのソースを参照する変数
        /// </summary>
        private AudioSource bgmSource;

        /// <summary>
        /// SE同時再生のためのAudioSourceプールを参照するリスト変数
        /// </summary>
        private List<AudioSource> seSourceList = new List<AudioSource>();

        /// <summary>
        /// 同時に鳴らせるSEの最大数
        /// </summary>
        public int maxSEChannels = 10;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // もしシングルトンではない場合
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();// 音響設定の初期化
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
            // BGMの初期化
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;

            // BGMデータをスキャン
            foreach (var bgm in bgmDataArray)
            {
                // もしディクショナリ変数が空の場合
                if (!bgmDict.ContainsKey(bgm.BGM_Type))
                {
                    bgmDict.Add(bgm.BGM_Type, bgm);// ディクショナリ変数にデータを追加
                }
            }

            // 存在しているSEのチャンネル分ループ
            for (int i = 0; i < maxSEChannels; i++)
            {
                // SEの初期化（チャンネルの確保）
                AudioSource seSource = gameObject.AddComponent<AudioSource>();
                seSource.playOnAwake = false;
                seSourceList.Add(seSource);// リストにSEソースを追加
            }

            // SEのデータをスキャン
            foreach (var se in seDataArray)
            {
                // もしディクショナリ変数が空の場合
                if (!seDict.ContainsKey(se.SE_Type))
                {
                    seDict.Add(se.SE_Type, se);// ディクショナリ変数にデータを追加
                }
            }
        }

        /// <summary>
        /// BGMを再生する関数
        /// </summary>
        public void PlayBGM(BGM_Type type)
        {
            // もしディクショナリ変数に中身がある場合
            if (bgmDict.TryGetValue(type, out BGM_Data data))
            {
                // もし同じBGMが既に鳴っている場合
                if (bgmSource.clip == data.Clip && bgmSource.isPlaying)
                {
                    return;
                }

                // BGMソースを指定の設定にして再生
                bgmSource.clip = data.Clip;
                bgmSource.volume = data.Volume;
                bgmSource.Play();
            }
        }

        /// <summary>
        /// BGMを停止する関数
        /// </summary>
        public void StopBGM() => bgmSource.Stop();

        /// <summary>
        /// SEを再生する関数
        /// </summary>
        public void PlaySE(SE_Type type)
        {
            // もしディクショナリ変数に中身がある場合
            if (seDict.TryGetValue(type, out SE_Data data))
            {
                // 空いているAudioSource（再生中でないもの）を探して鳴らす
                foreach (var source in seSourceList)
                {
                    // もしプレイ中のソースが無い場合
                    if (!source.isPlaying)
                    {
                        // SEソースを指定の設定にして再生
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
            PlaySE(SE_Type.Cursor);// 指定の音を再生
        }
    }
}
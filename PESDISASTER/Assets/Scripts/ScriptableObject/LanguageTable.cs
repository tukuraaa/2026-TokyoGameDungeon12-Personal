using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1つのテキスト項目のデータ構造を定義するクラス
/// </summary>
[System.Serializable]
public struct LanguageEntry
{
    /// <summary>
    /// テキスト項目のキーコードを参照する変数
    /// </summary>
    public string KeyCode;
    /// <summary>
    /// 英語のテキストを参照する変数
    /// </summary>
    public string English;
    /// <summary>
    /// 日本語のテキストを参照する変数
    /// </summary>
    public string Japanese;
    /// <summary>
    /// 中国語のテキストを参照する変数
    /// </summary>
    public string Chinese;
}

/// <summary>
/// データの入れ物を定義するクラス
/// </summary>
[CreateAssetMenu(fileName = "LanguageTable", menuName = "Localization/LanguageTable")]
public class LanguageTable : ScriptableObject
{
    /// <summary>
    /// テキスト項目のリストを参照する変数
    /// </summary>
    public List<LanguageEntry> TextList = new List<LanguageEntry>();

    /// <summary>
    /// 指定されたキーコードと言語コードに基づいて、対応するテキストを取得する関数
    /// </summary>
    /// <param name="keyCode"></param>
    /// <param name="languageCode"></param>
    /// <returns></returns>
    public string GetText(string keyCode, string languageCode)
    {
        // テキスト項目のリストから、指定されたキーコードに一致するテキストを検索し参照する変数を定義する
        var entryText = TextList.Find(x => x.KeyCode == keyCode);

        // 言語コードに基づいて、対応するテキストを返す
        return languageCode switch
        {
            // 言語コードが "English" の場合、英語のテキストを返す
            "English" => entryText.English,
            // 言語コードが "Japanese" の場合、日本語のテキストを返す
            "Japanese" => entryText.Japanese,
            // 言語コードが "Chinese" の場合、中国語のテキストを返す
            "Chinese" => entryText.Chinese,
            // デフォルトの場合、英語のテキストを返す
            _ => entryText.English
        };
    }
}
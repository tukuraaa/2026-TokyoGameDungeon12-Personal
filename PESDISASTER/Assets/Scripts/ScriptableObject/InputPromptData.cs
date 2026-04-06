using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace PESDISASTER
{
    /// <summary>
    /// データベース全体を定義するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "NewInputPromptData", menuName = "PESDISASTER/InputPromptData")]
    public class InputPromptData : ScriptableObject
    {
        /// <summary>
        /// キーと画像のセットを定義する構造体
        /// </summary>
        [System.Serializable]
        public struct KeyPromptPair
        {
            /// <summary>
            /// 表示する画像を参照する変数
            /// </summary>
            public Sprite promptSprite;

            /// <summary>
            /// 対象のキーを参照する変数
            /// </summary>
            public Key key;
        }

        /// <summary>
        /// キーと画像のセットのリストを参照する変数
        /// </summary>
        public List<KeyPromptPair> keyboardPrompts;

        /// <summary>
        /// キーから画像を検索する関数
        /// </summary>
        /// <param name="targetKey"></param>
        /// <returns></returns>
        public Sprite GetSprite(Key targetKey)
        {
            // リスト内をすべて検索
            foreach (var pair in keyboardPrompts)
            {
                // もし対象のキーと一致するペアが見つかった場合
                if (pair.key == targetKey)
                {
                    return pair.promptSprite;// そのペアの画像を返す
                }
            }

            return null;// 見つからなかった場合はnullを返す
        }
    }
}
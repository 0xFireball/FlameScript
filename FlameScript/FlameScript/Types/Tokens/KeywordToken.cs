using System;
using System.Collections.Generic;

namespace FlameScript.Types.Tokens
{
    public class KeywordToken : Token
    {
        private static readonly Dictionary<string, KeywordType> validKeywords = new Dictionary<string, KeywordType>()
        {
            { "if", KeywordType.If },
            { "num", KeywordType.Number },
            { "return", KeywordType.Return },
            { "void", KeywordType.Void },
            { "while", KeywordType.While },
        };

        private static readonly Dictionary<KeywordType, VariableType> keywordTypeToVariableTypeMappings = new Dictionary<KeywordType, VariableType>
        {
            { KeywordType.Number, VariableType.Number },
            { KeywordType.Void, VariableType.Void },
        };

        public KeywordType KeywordType { get; private set; }

        /// <summary>
        /// Returns true, if this keyword is a keyword
        /// for a type, false otherwise.
        /// </summary>
        public bool IsTypeKeyword
        {
            get { return keywordTypeToVariableTypeMappings.ContainsKey(KeywordType); }
        }

        public KeywordToken(string name)
            : base(name)
        {
            if (!validKeywords.ContainsKey(name))
                throw new ArgumentException("The name is not a valid keyword.", nameof(name));

            KeywordType = validKeywords[name];
        }

        /// <summary>
        /// Returns true, if the given string is a known
        /// keyword, false otherwise.
        /// </summary>
        public static bool IsKeyword(string s)
        {
            return validKeywords.ContainsKey(s);
        }

        /// <summary>
        /// Returns the assisated VariableType for this keyword,
        /// if this keyword represents a variable type.
        /// Throws an excepion otherwise.
        /// </summary>
        public VariableType ToVariableType()
        {
            return keywordTypeToVariableTypeMappings[KeywordType];
        }
    }
}
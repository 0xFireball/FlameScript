using FlameScript.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlameScript.Lexing
{
    /// <summary>
    /// FlameScript Tokenizer
    /// </summary>
    public class Tokenizer
    {
        /// <summary>
        /// The raw input code
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Current position of the pointer in reading the code
        /// </summary>
        private int _readingPosition;

        public Tokenizer(string code)
        {
            Code = code;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            var builder = new StringBuilder();

            while (!EndOfCode)
            {
                SkipCharacter(CharType.WhiteSpace);
            }
        }

        private void SkipCharacter(CharType charTypeToSkip)
        {
            while (PeekNextCharacterType().HasAnyFlag(charTypeToSkip))
                next();
        }

        private CharType PeekNextCharacterType() => PeekNextCharacter().GetCharType();

        /// <summary>
        /// Returns the next character without advancing the pointer.
        /// </summary>
        /// <returns></returns>
        private char PeekNextCharacter() => Code[_readingPosition];

        /// <summary>
        /// Returns the next character and advances the pointer.
        /// </summary>
        private char NextCharacter()
        {
            var ret = PeekNextCharacter();
            _readingPosition++;
            return ret;
        }

        public bool EndOfCode => _readingPosition >= Code.Length;
    }
}
using FlameScript.Types;
using FlameScript.Types.Tokens;
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

        public Token[] Tokenize()
        {
            var tokens = new List<Token>();
            var builder = new StringBuilder();

            while (!EndOfCode)
            {
                SkipCharacter(CharType.WhiteSpace);
                switch (PeekNextCharacterType())
                {
                    case CharType.Alpha: //start of identifier
                        ReadToken(builder, CharType.AlphaNumeric);
                        string s = builder.ToString();
                        if (KeywordToken.IsKeyword(s))
                            tokens.Add(new KeywordToken(s));
                        else
                            tokens.Add(new IdentifierToken(s));
                        builder.Clear();
                        break;

                    case CharType.Numeric: //start of number literal
                        ReadToken(builder, CharType.Numeric);
                        tokens.Add(new NumberLiteralToken(builder.ToString()));
                        builder.Clear();
                        break;

                    case CharType.Operator:
                        ReadToken(builder, CharType.Operator);
                        tokens.Add(new OperatorToken(builder.ToString()));
                        builder.Clear();
                        break;

                    case CharType.OpenBrace:
                        tokens.Add(new OpenBraceToken(NextCharacter().ToString()));
                        break;

                    case CharType.CloseBrace:
                        tokens.Add(new CloseBraceToken(NextCharacter().ToString()));
                        break;

                    case CharType.ArgSeperator:
                        tokens.Add(new ArgSeperatorToken(NextCharacter().ToString()));
                        break;

                    case CharType.StatementSeperator:
                        tokens.Add(new StatementSperatorToken(NextCharacter().ToString()));
                        break;

                    default:
                        throw new Exception("The tokenizer found an unidentifiable character.");
                }
            }

            return tokens.ToArray();
        }

        /// <summary>
        /// Reads characters into the StringBuilder while they match
        /// the given type(s).
        /// </summary>
        private void ReadToken(StringBuilder builder, CharType charTypeToRead)
        {
            while (!EndOfCode && PeekNextCharacterType().HasAnyFlag(charTypeToRead))
            {
                builder.Append(NextCharacter());
            }
        }

        private void SkipCharacter(CharType charTypeToSkip)
        {
            while (PeekNextCharacterType().HasAnyFlag(charTypeToSkip))
                NextCharacter();
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
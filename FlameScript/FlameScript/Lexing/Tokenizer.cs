﻿using FlameScript.Types;
using FlameScript.Types.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
        public string Code { get; private set; }

        /// <summary>
        /// Current position of the pointer in reading the code
        /// </summary>
        private int _readingPosition;

        public Tokenizer(string code)
        {
            Code = code;
        }

        /// <summary>
        /// Parses the input code and returns an array of Token objects.
        /// </summary>
        /// <returns></returns>
        public Token[] Tokenize()
        {
            var tokens = new List<Token>();
            var builder = new StringBuilder();

            Code = StripCommentsAndNormalizeNewlines(Code);

            while (!EndOfCode)
            {
                SkipCharacter(CharType.WhiteSpace);
                var nextChar = PeekNextCharacter();
                var nextCharType = nextChar.GetCharType();
                switch (nextCharType)
                {
                    case CharType.Alpha: //start of identifier
                        ReadTokens(builder, CharType.AlphaNumeric);
                        string s = builder.ToString();
                        if (KeywordToken.IsKeyword(s))
                            tokens.Add(new KeywordToken(s));
                        else
                            tokens.Add(new IdentifierToken(s));
                        builder.Clear();
                        break;

                    case CharType.MemberAccess: //start of member access
                        NextCharacter(); //Skip the period
                        ReadTokens(builder, CharType.AlphaNumeric);
                        tokens.Add(new MemberAccessToken(builder.ToString()));
                        builder.Clear();
                        //TODO: Find an alternative method
                        //Patch the previous token as a TableReferenceToken
                        var previousToken = tokens[tokens.Count - 2]; //second-to-last element
                        if (previousToken is IdentifierToken)
                        {
                            previousToken = new TableIdentifierToken(previousToken.Content);
                            tokens[tokens.Count - 2] = previousToken;
                        }
                        break;

                    case CharType.StringDelimiter:
                        NextCharacter(); //Skip the opening quote
                        ReadTokensUntil(builder, CharType.StringDelimiter);
                        NextCharacter(); //Skip the ending quote
                        tokens.Add(new StringLiteralToken(builder.ToString()));
                        builder.Clear();
                        break;

                    case CharType.Numeric: //start of number literal, allow for decimal numbers too
                        ReadTokens(builder, CharType.DecimalNumeric);
                        tokens.Add(new NumberLiteralToken(builder.ToString()));
                        builder.Clear();
                        break;

                    case CharType.Operator:
                        //It is an operator
                        ReadTokens(builder, CharType.Operator);
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
                        tokens.Add(new StatementSeparatorToken(NextCharacter().ToString()));
                        break;

                    default:
                        throw new Exception($"The tokenizer found an unidentifiable character: {nextChar}");
                }
            }

            return tokens.ToArray();
        }

        /// <summary>
        /// A method that strips comments from the source and normalizes newlines to LF. Thank you http://stackoverflow.com/questions/3524317/regex-to-strip-line-comments-from-c-sharp/3524689#3524689 for the regex patterns.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string StripCommentsAndNormalizeNewlines(string input)
        {
            var blockComments = @"/\*(.*?)\*/";
            var lineComments = @"//(.*?)\r?\n";
            var strings = @"""((\\[^\n]|[^""\n])*)""";
            var verbatimStrings = @"@(""[^""]*"")+";
            input = input.Replace("\r\n", "\n"); //Convert CRLF to LF
            return Regex.Replace(input,
                blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
                me =>
                {
                    if (me.Value.StartsWith("/*", StringComparison.InvariantCulture) || me.Value.StartsWith("//", StringComparison.InvariantCulture))
                    {
                        /*
                        var retVal = me.Value.StartsWith("//", StringComparison.InvariantCulture) ? Environment.NewLine : "";
                        return retVal;
                        */
                        var retVal = string.Concat(Enumerable.Repeat('\n', me.Value.Count(ch => ch == '\n')));
                        return retVal;
                    }

                    // Keep the literal strings
                    return me.Value;
                },
                RegexOptions.Singleline);
        }

        /// <summary>
        /// Reads characters into the StringBuilder while they match
        /// the given type(s).
        /// </summary>
        private void ReadTokens(StringBuilder builder, CharType charTypeToRead)
        {
            while (!EndOfCode && PeekNextCharacterType().HasAnyFlag(charTypeToRead))
            {
                builder.Append(NextCharacter());
            }
        }

        /// <summary>
        /// Reads characters into the StringBuilder while they match
        /// the given type(s).
        /// </summary>
        private void ReadTokensUntil(StringBuilder builder, CharType charTypeToStopAt)
        {
            var upcomingCharacterType = PeekNextCharacterType();
            while (!EndOfCode && !upcomingCharacterType.HasAnyFlag(charTypeToStopAt))
            {
                builder.Append(NextCharacter());
                upcomingCharacterType = PeekNextCharacterType();
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
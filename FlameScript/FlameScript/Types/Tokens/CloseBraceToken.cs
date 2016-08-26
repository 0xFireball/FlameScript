using System;

namespace FlameScript.Types.Tokens
{
    public class CloseBraceToken : BraceToken
    {
        public CloseBraceToken(string symbol)
            : base(symbol)
        {
            switch (symbol)
            {
                case ")":
                    BraceType = BraceType.Round;
                    break;

                case "]":
                    BraceType = BraceType.Square;
                    break;

                case "}":
                    BraceType = BraceType.Curly;
                    break;

                default:
                    throw new ArgumentException("The symbol is not a closing brace.", nameof(symbol));
            }
        }
    }
}
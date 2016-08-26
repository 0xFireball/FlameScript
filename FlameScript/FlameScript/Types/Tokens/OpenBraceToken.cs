using System;

namespace FlameScript.Types.Tokens
{
    public class OpenBraceToken : BraceToken
    {
        public OpenBraceToken(string symbol)
            : base(symbol)
        {
            switch (symbol)
            {
                case "(":
                    BraceType = BraceType.Round;
                    break;

                case "[":
                    BraceType = BraceType.Square;
                    break;

                case "{":
                    BraceType = BraceType.Curly;
                    break;

                default:
                    throw new ArgumentException("The symbol is not an opening brace.", nameof(symbol));
            }
        }
    }
}
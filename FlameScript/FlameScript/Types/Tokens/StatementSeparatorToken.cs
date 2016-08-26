using System;

namespace FlameScript.Types.Tokens
{
    public class StatementSperatorToken : Token
    {
        public StatementSperatorToken(string symbol)
            : base(symbol)
        {
            if (symbol != ";")
                throw new ArgumentException("The symbol is not statement seperator.", nameof(symbol));
        }
    }
}
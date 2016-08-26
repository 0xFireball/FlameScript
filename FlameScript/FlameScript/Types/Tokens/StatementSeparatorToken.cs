using System;

namespace FlameScript.Types.Tokens
{
    public class StatementSeparatorToken : Token
    {
        public StatementSeparatorToken(string symbol)
            : base(symbol)
        {
            if (symbol != ";")
                throw new ArgumentException("The symbol is not statement seperator.", nameof(symbol));
        }
    }
}
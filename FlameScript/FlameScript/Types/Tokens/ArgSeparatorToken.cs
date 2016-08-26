using System;

namespace FlameScript.Types.Tokens
{
    public class ArgSeperatorToken : Token
    {
        public ArgSeperatorToken(string symbol)
            : base(symbol)
        {
            if (symbol != ",")
                throw new ArgumentException("The symbol is not an argument seperator.", nameof(symbol));
        }
    }
}
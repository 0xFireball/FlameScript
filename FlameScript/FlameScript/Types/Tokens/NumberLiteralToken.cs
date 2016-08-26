using System;

namespace FlameScript.Types.Tokens
{
    public class NumberLiteralToken : Token
    {
        public int Number
        {
            get
            {
                return number;
            }
        }

        private int number;

        public NumberLiteralToken(string name)
            : base(name)
        {
            if (!int.TryParse(name, out number))
                throw new ArgumentException("The name is not a valid number literal.", nameof(name));
        }
    }
}
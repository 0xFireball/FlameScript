using System;

namespace FlameScript.Types.Tokens
{
    public class NumberLiteralToken : Token
    {
        public double Number
        {
            get
            {
                return number;
            }
        }

        private double number;

        public NumberLiteralToken(string name)
            : base(name)
        {
            if (!double.TryParse(name, out number))
                throw new ArgumentException("The name is not a valid number literal.", nameof(name));
        }
    }
}
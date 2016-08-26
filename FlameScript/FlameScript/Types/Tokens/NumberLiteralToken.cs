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

        public NumberLiteralToken(string content)
            : base(content)
        {
            if (!double.TryParse(content, out number))
                throw new ArgumentException("The name is not a valid number literal.", nameof(content));
        }
    }
}
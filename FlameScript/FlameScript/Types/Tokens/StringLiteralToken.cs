namespace FlameScript.Types.Tokens
{
    public class StringLiteralToken : Token
    {
        public string String { get; }

        public StringLiteralToken(string str) : base(str)
        {
            String = str;
        }
    }
}
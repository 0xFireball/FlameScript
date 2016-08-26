namespace FlameScript.Types.Tokens
{
    public class StringLiteralToken : LiteralToken
    {
        public string String { get; }

        public StringLiteralToken(string str) : base(str)
        {
            String = str;
            Content = $"\"{String}\""; //The token only contains string value, this allows to preserve ability to rebuild code from tokens
        }
    }
}
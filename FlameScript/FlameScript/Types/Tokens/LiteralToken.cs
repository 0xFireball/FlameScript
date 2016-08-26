namespace FlameScript.Types.Tokens
{
    /// <summary>
    /// A token that represents a literal value
    /// </summary>
    public class LiteralToken : Token
    {
        public LiteralToken(string content) : base(content)
        {
        }
    }
}
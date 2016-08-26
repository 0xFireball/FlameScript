namespace FlameScript.Types.Tokens
{
    public abstract class BraceToken : Token
    {
        public BraceType BraceType { get; protected set; }

        public BraceToken(string content) : base(content)
        {

        }
    }
}
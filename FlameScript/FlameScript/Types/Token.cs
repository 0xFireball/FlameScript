namespace FlameScript.Types
{
    public abstract class Token
    {
        public string Content { get; protected set; }

        public Token(string content)
        {
            Content = content;
        }
    }
}
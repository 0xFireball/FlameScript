namespace FlameScript.Types
{
    public abstract class Token
    {
        public string Content { get; }

        public Token(string content)
        {
            Content = content;
        }
    }
}
namespace FlameScript.Types.Tokens
{
    public class MemberAccessToken : Token
    {
        public string MemberName;

        public MemberAccessToken(string content) : base($".{content}") //Add the period before
        {
            MemberName = content;
        }
    }
}
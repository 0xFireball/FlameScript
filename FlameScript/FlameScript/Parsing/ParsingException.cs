using System;

namespace FlameScript.Parsing
{
    /// <summary>
    /// An exception that occured while parsing the tokenized code. You can retrieve the position of the error from the CodePosition property
    /// </summary>
    public class ParsingException : Exception
    {
        public CodePosition ErrorPosition { get; }

        public ParsingException()
        {
        }

        public ParsingException(string message) : base(message)
        {
        }

        public ParsingException(string message, Exception inner) : base(message, inner)
        {
        }

        public ParsingException(string message, CodePosition position) : this(message)
        {
            ErrorPosition = position;
        }
    }
}
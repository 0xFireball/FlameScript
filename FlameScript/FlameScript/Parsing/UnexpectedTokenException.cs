using FlameScript.Types;
using System;

namespace FlameScript.Parsing
{
    public class UnexpectedTokenException : ParsingException
    {
        public Token ErrorToken { get; }

        public UnexpectedTokenException()
        {
        }

        public UnexpectedTokenException(string message) : base(message)
        {
        }

        public UnexpectedTokenException(string message, Exception inner) : base(message, inner)
        {
        }

        public UnexpectedTokenException(string message, CodePosition position) : base(message, position)
        {
        }

        public UnexpectedTokenException(string message, Token token) : base(message)
        {
            ErrorToken = token;
        }
    }
}
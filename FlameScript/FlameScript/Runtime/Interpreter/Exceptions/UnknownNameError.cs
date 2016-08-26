using System;
using System.Runtime.Serialization;

namespace FlameScript.Runtime.Interpreter.Exceptions
{
    public class UnknownNameError : Exception
    {
        public UnknownNameError()
        {
        }

        public UnknownNameError(string message) : base(message)
        {
        }

        public UnknownNameError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnknownNameError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace FlameScript.Runtime.Interpreter.Exceptions
{
    public class UnknownNameError : Exception
    {
        public string VariableName { get; }

        [Obsolete]
        public UnknownNameError()
        {
        }

        public UnknownNameError(string message, string variableName) : base(message)
        {
            VariableName = variableName;
        }

        [Obsolete]
        public UnknownNameError(string message, Exception innerException) : base(message, innerException)
        {
        }

        [Obsolete]
        protected UnknownNameError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
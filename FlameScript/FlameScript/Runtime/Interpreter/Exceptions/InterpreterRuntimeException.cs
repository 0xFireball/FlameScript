using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlameScript.Runtime.Interpreter.Exceptions
{
    public class InterpreterRuntimeException : Exception
    {
        public InterpreterRuntimeException()
        {
        }

        public InterpreterRuntimeException(string message) : base(message)
        {
        }

        public InterpreterRuntimeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InterpreterRuntimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

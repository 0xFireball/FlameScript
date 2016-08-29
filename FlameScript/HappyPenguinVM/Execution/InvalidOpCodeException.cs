using HappyPenguinVM.Types;
using System;

namespace HappyPenguinVM.Execution
{
    [Serializable]
    public class InvalidOpCodeException : Exception
    {
        public OpCode ErrorOpCode { get; }

        [Obsolete]
        public InvalidOpCodeException()
        {
        }

        [Obsolete]
        public InvalidOpCodeException(string message) : base(message)
        {
        }

        public InvalidOpCodeException(string message, OpCode errorOpCode) : base(message)
        {
            ErrorOpCode = errorOpCode;
        }

        [Obsolete]
        public InvalidOpCodeException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidOpCodeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
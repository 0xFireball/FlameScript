using HappyPenguinVM.Types;
using System.Collections.Generic;

namespace FlameScript.Compiler
{
    /// <summary>
    /// Code emitter for HappyPenguinVM machine code.
    /// </summary>
    internal class HappyPenguinCodeEmitter
    {
        private List<CodeInstruction> _emittedCode = new List<CodeInstruction>();

        public void Emit(OpCode code)
        {
            Emit(code, 0, 0);
        }

        public void Emit(OpCode code, byte arg1)
        {
            Emit(code, arg1, 0);
        }

        public void Emit(OpCode code, byte arg1, byte arg2)
        {
            _emittedCode.Add(new CodeInstruction { OpCode = code, ByteArg1 = arg1, ByteArg2 = arg2 });
        }

        public void Emit(OpCode code, short arg)
        {
            _emittedCode.Add(new CodeInstruction { OpCode = code, ShortArg = arg });
        }

        public CodeInstruction[] GetEmittedCode()
        {
            return _emittedCode.ToArray();
        }
    }
}
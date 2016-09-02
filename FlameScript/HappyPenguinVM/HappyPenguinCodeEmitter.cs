using HappyPenguinVM.Types;
using System.Collections.Generic;

namespace HappyPenguinVM
{
    /// <summary>
    /// Code emitter for HappyPenguinVM machine code.
    /// </summary>
    public class HappyPenguinCodeEmitter
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

        public void Emit(OpCode code, ushort arg)
        {
            _emittedCode.Add(new CodeInstruction { OpCode = code, UShortArg1 = arg });
        }

        public void Emit(OpCode code, ushort arg1, ushort arg2)
        {
            _emittedCode.Add(new CodeInstruction { OpCode = code, UShortArg1 = arg1, UShortArg2 = arg2 });
        }

        public void Emit(OpCode code, uint arg)
        {
            _emittedCode.Add(new CodeInstruction { OpCode = code, UIntArg1 = arg });
        }

        public void Emit(OpCode code, uint arg1, uint arg2)
        {
            _emittedCode.Add(new CodeInstruction { OpCode = code, UIntArg1 = arg1, UIntArg2 = arg2 });
        }

        public HappyPenguinVMProgram GetEmittedCode()
        {
            return new HappyPenguinVMProgram(_emittedCode);
        }
    }
}
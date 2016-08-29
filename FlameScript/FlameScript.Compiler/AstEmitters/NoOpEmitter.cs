using HappyPenguinVM;
using HappyPenguinVM.Types;

namespace FlameScript.Compiler.AstEmitters
{
    public class NoOpEmitter : StatementSequenceEmitter
    {
        public override void EmitCode(HappyPenguinCodeEmitter emitter)
        {
            emitter.Emit(OpCode.Nop);
        }
    }
}
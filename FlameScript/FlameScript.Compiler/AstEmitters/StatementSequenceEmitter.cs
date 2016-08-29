using System.Collections.Generic;

namespace FlameScript.Compiler.AstEmitters
{
    public abstract class StatementSequenceEmitter : AstEmitter
    {
        public List<StatementSequenceEmitter> SubNodes { get; } = new List<StatementSequenceEmitter>();

        public abstract void EmitCode(HappyPenguinCodeEmitter emitter);
    }
}
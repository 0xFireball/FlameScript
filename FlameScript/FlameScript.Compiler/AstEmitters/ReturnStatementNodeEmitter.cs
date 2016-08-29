using FlameScript.Types.Ast;
using HappyPenguinVM.Types;

namespace FlameScript.Compiler.AstEmitters
{
    internal class ReturnStatementNodeEmitter : StatementSequenceEmitter
    {
        private ReturnStatementNode returnStatementNode;

        public ReturnStatementNodeEmitter(ReturnStatementNode returnStatementNode)
        {
            this.returnStatementNode = returnStatementNode;
        }

        public override void EmitCode(HappyPenguinCodeEmitter emitter)
        {
            emitter.Emit(OpCode.Return);
        }
    }
}
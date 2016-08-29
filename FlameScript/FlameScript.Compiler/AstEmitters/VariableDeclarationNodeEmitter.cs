using System;
using FlameScript.Types.Ast;

namespace FlameScript.Compiler.AstEmitters
{
    internal class VariableDeclarationNodeEmitter : StatementSequenceEmitter
    {
        private VariableDeclarationNode variableDeclarationNode;

        public VariableDeclarationNodeEmitter(VariableDeclarationNode variableDeclarationNode)
        {
            this.variableDeclarationNode = variableDeclarationNode;
        }

        public override void EmitCode(HappyPenguinCodeEmitter emitter)
        {
            throw new NotImplementedException();
        }
    }
}
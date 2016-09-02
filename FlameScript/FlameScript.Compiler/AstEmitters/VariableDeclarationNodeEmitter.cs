using FlameScript.Types.Ast;
using HappyPenguinVM;
using System;

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
            //TODO: Emit code for variable declaration
            //throw new NotImplementedException();
        }
    }
}
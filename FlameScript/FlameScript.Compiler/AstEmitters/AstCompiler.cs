using FlameScript.Compiler.Util;
using FlameScript.Types.Ast;

namespace FlameScript.Compiler.AstEmitters
{
    public class AstCompiler
    {
        private ProgramNode _programAst;

        public AstCompiler(ProgramNode ast)
        {
            this._programAst = ast;
        }

        public ProgramNodeEmitter CreateEmitter()
        {
            var rootEmitter = new ProgramNodeEmitter();
            AddEmitterToRoot(rootEmitter, _programAst);
            return rootEmitter;
        }

        private void AddEmitterToRoot(StatementSequenceEmitter rootEmitter, StatementSequenceNode astNode)
        {
            foreach (var astSubNode in astNode.SubNodes)
            {
                StatementSequenceEmitter emittedNode = new NoOpEmitter();
                TypeSwitch.On(astSubNode)
                    .Case((VariableDeclarationNode variableDeclarationNode) =>
                    {
                        emittedNode = new VariableDeclarationNodeEmitter(variableDeclarationNode);
                    });
                if (astSubNode is StatementSequenceNode)
                    AddEmitterToRoot(emittedNode, astSubNode as StatementSequenceNode); //Also copy subnodes
                rootEmitter.SubNodes.Add(emittedNode);
            }
        }
    }
}
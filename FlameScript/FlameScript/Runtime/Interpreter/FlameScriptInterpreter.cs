using FlameScript.Types.Ast;
using System.Collections.Generic;
using System.Linq;

namespace FlameScript.Runtime.Interpreter
{
    public class FlameScriptInterpreter
    {
        public ProgramNode SyntaxTree;

        public List<FunctionDeclarationNode> Methods;

        public FlameScriptInterpreter(ProgramNode syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }

        public void ExecuteProgram()
        {
            //Build method table
            Methods = new List<FunctionDeclarationNode>();
            Methods.AddRange(SyntaxTree.SubNodes.Where(subNode => subNode is FunctionDeclarationNode).Cast<FunctionDeclarationNode>());

            //Get the entry point of the program
            var entryPointFunction = Methods.Where(function => function.FunctionName == "main");

            //Begin execution on global scope
            var nonMethodNodes = SyntaxTree.SubNodes.Except(Methods);

            //TODO: Execute all non-method nodes on the global scope
            //TODO: Begin normal execution at the entry point
        }
    }
}
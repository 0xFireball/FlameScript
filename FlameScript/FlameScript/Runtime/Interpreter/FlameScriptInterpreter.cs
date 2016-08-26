using FlameScript.Runtime.Interpreter.Exceptions;
using FlameScript.Runtime.Interpreter.Types;
using FlameScript.Types;
using FlameScript.Types.Ast;
using System.Collections.Generic;
using System.Linq;

namespace FlameScript.Runtime.Interpreter
{
    public class FlameScriptInterpreter
    {
        public ProgramNode SyntaxTree;

        public List<FunctionDeclarationNode> Methods;
        public List<Variable> Variables;

        public FlameScriptInterpreter(ProgramNode syntaxTree)
        {
            SyntaxTree = syntaxTree;

            Variables = new List<Variable>();
            Methods = new List<FunctionDeclarationNode>();
        }

        public void ExecuteProgram()
        {
            //Build method table

            Methods.AddRange(SyntaxTree.SubNodes.Where(subNode => subNode is FunctionDeclarationNode).Cast<FunctionDeclarationNode>());
            //TODO: Throw errors on duplicates, etc.

            //Get the entry point of the program
            var entryPointMethodCandidates = Methods.Where(function => function.FunctionName == "main").ToList();

            if (entryPointMethodCandidates.Count == 0)
                throw new InterpreterRuntimeException("No entry point method could be found.");
            var entryPointMethod = entryPointMethodCandidates[0];

            //Begin execution on global scope
            var nonMethodDeclarationNodes = SyntaxTree.SubNodes.Except(Methods).ToList();

            //TODO: Execute all non-method declaration nodes on the global scope
            ExecuteNodes(nonMethodDeclarationNodes);

            //TODO: Begin normal execution at the entry point
            ExecuteNodes(entryPointMethod.SubNodes.ToList());
        }

        private void ExecuteNodes(List<AstNode> nonMethodDeclarationNodes)
        {
            foreach (var nodeToExecute in nonMethodDeclarationNodes)
            {
                TypeSwitch.On(nodeToExecute)
                    .Case((VariableDeclarationNode node) =>
                    {
                        var newVar = new Variable { Name = node.Name, Value = EvaluateExpression(node.InitialValueExpression) };
                        Variables.Add(newVar);
                    })
                    .Case((VariableAssignmentNode node) =>
                    {
                        var matchedVariables = Variables.Where(var => var.Name == node.VariableName).ToList();
                        if (matchedVariables.Count == 1)
                        {
                            var currentVariable = matchedVariables[0];
                            currentVariable.Value = EvaluateExpression(node.ValueExpression);
                        }
                        else
                        {
                            throw new UnknownNameError("No variable with the given name exists.", node.VariableName);
                        }
                    });
            }
        }

        private dynamic EvaluateExpression(ExpressionNode valueExpression)
        {
            if (valueExpression is NumberLiteralNode) //Easy, it's a number
            {
                var expressionResult = valueExpression as NumberLiteralNode;
                return expressionResult.Value;
            }
            else if (valueExpression is BinaryOperationNode)
            {
                var expressionOperation = valueExpression as BinaryOperationNode;
                return DoBinaryOperation(expressionOperation);
            }
            else if (valueExpression is VariableReferenceExpressionNode)
            {
                var variableReference = valueExpression as VariableReferenceExpressionNode;
                var matchedVariables = Variables.Where(var => var.Name == variableReference.VariableName).ToList();
                if (matchedVariables.Count == 1)
                {
                    var currentVariable = matchedVariables[0];
                    return currentVariable.Value;
                }
                else
                {
                    throw new UnknownNameError("No variable with the given name exists.", variableReference.VariableName);
                }
            }
            //Error evaluating expression (should really throw an error or something)
            return null;
        }

        private dynamic DoBinaryOperation(BinaryOperationNode expressionOperation)
        {
            //TODO: Implement all of them
            switch (expressionOperation.OperationType)
            {
                case ExpressionOperationType.Add:
                    return EvaluateExpression(expressionOperation.OperandA) + EvaluateExpression(expressionOperation.OperandB);
                    //TODO: Implement the rest of them
            }
            //Error doing operation (should really throw an error or something)
            return null;
        }
    }
}
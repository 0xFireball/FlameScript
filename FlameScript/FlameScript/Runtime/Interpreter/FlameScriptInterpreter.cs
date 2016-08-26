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
        public List<Variable> GlobalVariables;

        private Stack<List<Variable>> MethodScopeVariables;
        private List<Variable> CurrentContextScopeVariables => MethodScopeVariables.Peek();
        private List<Variable> CurrentVariablesInScope => GetCurrentVariablesInScope();

        private VariableScope CurrentVariableScope { get; set; }

        private List<Variable> GetCurrentVariablesInScope()
        {
            if (CurrentContextScopeVariables != null)
                return CurrentContextScopeVariables.Concat(GlobalVariables).ToList();
            else
                return GlobalVariables;
        }

        public FlameScriptInterpreter(ProgramNode syntaxTree)
        {
            SyntaxTree = syntaxTree;

            GlobalVariables = new List<Variable>();
            Methods = new List<FunctionDeclarationNode>();

            MethodScopeVariables = new Stack<List<Variable>>();
            MethodScopeVariables.Push(new List<Variable>()); //Buffer layer
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
            CurrentVariableScope = VariableScope.Global;
            ExecuteNodes(nonMethodDeclarationNodes);

            //TODO: Begin normal execution at the entry point
            CurrentVariableScope = VariableScope.Local;

            //Create variable scope for main() method

            ExecuteNodes(entryPointMethod.SubNodes.ToList());
        }

        private void ExecuteNodes(List<AstNode> nonMethodDeclarationNodes)
        {
            foreach (var nodeToExecute in nonMethodDeclarationNodes)
            {
                TypeSwitch.On(nodeToExecute)
                    .Case((VariableDeclarationNode variableDeclarationNode) =>
                    {
                        var newVar = new Variable { Name = variableDeclarationNode.Name, Value = EvaluateExpression(variableDeclarationNode.InitialValueExpression) };
                        if (CurrentVariableScope == VariableScope.Global)
                            GlobalVariables.Add(newVar);
                        else
                            CurrentContextScopeVariables.Add(newVar);
                    })
                    .Case((FunctionCallExpressionNode functionCallNode) =>
                    {
                        var matchedMethods = Methods.Where(method => method.FunctionName == functionCallNode.FunctionName).ToList();
                        if (matchedMethods.Count == 1)
                        {
                            var currentMethod = matchedMethods[0];
                            //Create method scope variables
                            var methodArgumentVariables = new List<Variable>();

                            var methodCallArguments = functionCallNode.Arguments.ToList();
                            var methodSigArguments = currentMethod.Parameters.ToList();

                            //Create
                            var createdArgumentVariables = methodSigArguments.Zip(methodCallArguments, (sigArg, callArg) =>
                            {
                                var outputVariable = new Variable
                                {
                                    Name = sigArg.Name,
                                    Value = EvaluateExpression(callArg)
                                };
                                return outputVariable;
                            });

                            methodArgumentVariables.AddRange(createdArgumentVariables);

                            MethodScopeVariables.Push(methodArgumentVariables);
                            ExecuteNodes(currentMethod.SubNodes.ToList());
                            MethodScopeVariables.Pop();
                        }
                        else
                        {
                            throw new UnknownNameError("No method with the given name exists.", functionCallNode.FunctionName);
                        }
                    })
                    .Case((VariableAssignmentNode variableAssignmentNode) =>
                    {
                        var matchedVariables = CurrentVariablesInScope.Where(var => var.Name == variableAssignmentNode.VariableName).ToList();
                        if (matchedVariables.Count == 1)
                        {
                            var currentVariable = matchedVariables[0];
                            currentVariable.Value = EvaluateExpression(variableAssignmentNode.ValueExpression);
                        }
                        else
                        {
                            throw new UnknownNameError("No variable with the given name exists.", variableAssignmentNode.VariableName);
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
                var matchedVariables = CurrentVariablesInScope.Where(var => var.Name == variableReference.VariableName).ToList();
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
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
                        var currentMethod = GetReferencedMethod(functionCallNode);
                        //This will be a call without regard to return type
                        CreateMethodCall(functionCallNode, currentMethod);
                    })
                    .Case((ReturnStatementNode returnStatementNode) =>
                    {
                        //If there is a return statement, we're obviously in a method scope
                        CurrentContextScopeVariables.Add(new Variable { Name = "$return", Value = EvaluateExpression(returnStatementNode.ValueExpression) ?? null });
                    })
                    .Case((BinaryOperationNode binaryOperationNode) =>
                    {
                        DoBinaryOperation(binaryOperationNode);
                    })
                    .Case((VariableAssignmentNode variableAssignmentNode) =>
                    {
                        var currentVariable = GetReferencedVariable(variableAssignmentNode.VariableName);
                        currentVariable.Value = EvaluateExpression(variableAssignmentNode.ValueExpression);
                    });
            }
        }

        private Variable GetReferencedVariable(string variableName)
        {
            var matchedVariables = CurrentVariablesInScope.Where(var => var.Name == variableName).ToList();
            if (matchedVariables.Count > 0)
            {
                var currentVariable = matchedVariables[0];
                return currentVariable;
            }
            else
            {
                throw new UnknownNameError("No variable with the given name exists.", variableName);
            }
        }

        private FunctionDeclarationNode GetReferencedMethod(FunctionCallExpressionNode functionCallNode)
        {
            var matchedMethods = Methods.Where(method => method.FunctionName == functionCallNode.FunctionName).ToList();
            if (matchedMethods.Count == 1)
            {
                var currentMethod = matchedMethods[0];
                return currentMethod;
            }
            else
            {
                throw new UnknownNameError("No method with the given name exists.", functionCallNode.FunctionName);
            }
        }

        private Variable CreateMethodCall(FunctionCallExpressionNode functionCallNode, FunctionDeclarationNode currentMethod)
        {
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
            var returnedVariable = CurrentContextScopeVariables.Where(variable => variable.Name == "$return").ToList()[0];
            MethodScopeVariables.Pop();
            return returnedVariable;
        }

        private dynamic EvaluateExpression(ExpressionNode expressionNode)
        {
            if (expressionNode is NumberLiteralNode) //Easy, it's a number
            {
                var expressionResult = expressionNode as NumberLiteralNode;
                return expressionResult.Value;
            }
            else if (expressionNode is BinaryOperationNode)
            {
                var expressionOperation = expressionNode as BinaryOperationNode;
                return DoBinaryOperation(expressionOperation);
            }
            else if (expressionNode is VariableReferenceExpressionNode)
            {
                var variableReference = expressionNode as VariableReferenceExpressionNode;
                var currentVariable = GetReferencedVariable(variableReference.VariableName);
                return currentVariable.Value;
            }
            else if (expressionNode is FunctionCallExpressionNode)
            {
                var functionCallExpression = expressionNode as FunctionCallExpressionNode;
                var methodRef = GetReferencedMethod(functionCallExpression);
                var methodCallReturn = CreateMethodCall(functionCallExpression, methodRef);
                return methodCallReturn.Value;
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

                case ExpressionOperationType.Assignment:
                    var evaluatedValue = EvaluateExpression(expressionOperation.OperandB);
                    var targetVariableReferenceNode = expressionOperation.OperandA as VariableReferenceExpressionNode;
                    var targetVariable = GetReferencedVariable(targetVariableReferenceNode.VariableName);
                    targetVariable.Value = evaluatedValue;
                    break;
                    //TODO: Implement the rest of them
            }
            //Error doing operation (should really throw an error or something)
            return null;
        }
    }
}
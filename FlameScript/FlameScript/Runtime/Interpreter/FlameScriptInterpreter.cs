using FlameScript.Runtime.Interpreter.Exceptions;
using FlameScript.Runtime.Interpreter.Types;
using FlameScript.Types;
using FlameScript.Types.Ast;
using FlameScript.Types.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlameScript.Runtime.Interpreter
{
    public class FlameScriptInterpreter
    {
        public ProgramNode SyntaxTree;

        public List<FunctionDeclarationNode> Functions;
        public List<Variable> GlobalVariables;

        private Stack<List<Variable>> FunctionScopeVariables;
        private List<Variable> CurrentContextScopeVariables => FunctionScopeVariables.Peek();
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
            Functions = new List<FunctionDeclarationNode>();

            FunctionScopeVariables = new Stack<List<Variable>>();
            FunctionScopeVariables.Push(new List<Variable>()); //Buffer layer
        }

        public void ExecuteProgram()
        {
            //Build function table

            Functions.AddRange(SyntaxTree.SubNodes.Where(subNode => subNode is FunctionDeclarationNode).Cast<FunctionDeclarationNode>());
            //TODO: Throw errors on duplicates, etc.

            //Get the entry point of the program
            var entryPointFunctionCandidates = Functions.Where(function => function.FunctionName == "main").ToList();

            if (entryPointFunctionCandidates.Count == 0)
                throw new InterpreterRuntimeException("No entry point function could be found.");
            var entryPointFunction = entryPointFunctionCandidates[0];

            //Begin execution on global scope
            var nonFunctionDeclarationNodes = SyntaxTree.SubNodes.Except(Functions).ToList();

            //TODO: Execute all non-function declaration nodes on the global scope
            CurrentVariableScope = VariableScope.Global;
            ExecuteNodes(nonFunctionDeclarationNodes);

            //TODO: Begin normal execution at the entry point
            CurrentVariableScope = VariableScope.Local;

            //Create variable scope for main() function

            ExecuteNodes(entryPointFunction.SubNodes.ToList());
        }

        private void ExecuteNodes(List<AstNode> nonFunctionDeclarationNodes)
        {
            foreach (var nodeToExecute in nonFunctionDeclarationNodes)
            {
                TypeSwitch.On(nodeToExecute)
                    .Case((VariableDeclarationNode variableDeclarationNode) =>
                    {
                        var newVar = new Variable { Name = variableDeclarationNode.Name, Value = EvaluateExpression(variableDeclarationNode.InitialValueExpression) };
                        if (CurrentVariableScope == VariableScope.Global)
                            GlobalVariables.Add(newVar);
                        else
                            CurrentContextScopeVariables.Add(newVar);
                        if (variableDeclarationNode.Type == VariableType.Table)
                        {
                            newVar.Value = new Expando();
                        }
                    })
                    .Case((FunctionCallExpressionNode functionCallNode) =>
                    {
                        var currentFunction = GetReferencedFunction(functionCallNode);
                        //This will be a call without regard to return type
                        CreateFunctionCall(functionCallNode, currentFunction);
                    })
                    .Case((ReturnStatementNode returnStatementNode) =>
                    {
                        //If there is a return statement, we're obviously in a function scope
                        CurrentContextScopeVariables.Add(new Variable { Name = "$return", Value = EvaluateExpression(returnStatementNode.ValueExpression) ?? null });
                    })
                    .Case((BinaryOperationNode binaryOperationNode) =>
                    {
                        DoBinaryOperation(binaryOperationNode);
                    })
                    .Case((IfStatementNode ifStatementNode) =>
                    {
                        //This only runs if the condition is true
                        if (EvaluateToBoolean(EvaluateExpression(ifStatementNode.Condition)))
                        {
                            //Condition is true, execute conditional nodes
                            ExecuteNodes(ifStatementNode.SubNodes.ToList());
                        }
                    })
                    .Case((WhileLoopNode whileLoopNode) =>
                    {
                        while (EvaluateToBoolean(EvaluateExpression(whileLoopNode.Condition)))
                        {
                            //Condition is true, execute conditional nodes
                            ExecuteNodes(whileLoopNode.SubNodes.ToList());
                        }
                    })
                    .Case((TableAssignmentNode tableAssignmentNode) =>
                    {
                        var currentVariable = GetReferencedVariable(tableAssignmentNode.TableQualifier.Name);
                        // Take off the last member
                        var memberChainList = tableAssignmentNode.TableQualifier.MemberChain.ToList();
                        var lastMemberInChain = memberChainList.Last();
                        memberChainList.Remove(lastMemberInChain);
                        dynamic intermediateMember = currentVariable.Value;
                        //Take all except the last
                        var remainingMembers = memberChainList;
                        foreach (var member in remainingMembers)
                        {
                            intermediateMember = intermediateMember[member];
                        }
                        intermediateMember[lastMemberInChain] = EvaluateExpression(tableAssignmentNode.ValueExpression);
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

        private FunctionDeclarationNode GetReferencedFunction(FunctionCallExpressionNode functionCallNode)
        {
            var matchedFunctions = Functions.Where(function => function.FunctionName == functionCallNode.FunctionName).ToList();
            if (matchedFunctions.Count == 1)
            {
                var currentFunction = matchedFunctions[0];
                return currentFunction;
            }
            else
            {
                throw new UnknownNameError("No function with the given name exists.", functionCallNode.FunctionName);
            }
        }

        private Variable CreateFunctionCall(FunctionCallExpressionNode functionCallNode, FunctionDeclarationNode currentFunction)
        {
            //Create function scope variables
            var functionArgumentVariables = new List<Variable>();

            var functionCallArguments = functionCallNode.Arguments.ToList();
            var functionSigArguments = currentFunction.Parameters.ToList();

            //Create
            var createdArgumentVariables = functionSigArguments.Zip(functionCallArguments, (sigArg, callArg) =>
            {
                var outputVariable = new Variable
                {
                    Name = sigArg.Name,
                    Value = EvaluateExpression(callArg)
                };
                return outputVariable;
            });

            functionArgumentVariables.AddRange(createdArgumentVariables);

            FunctionScopeVariables.Push(functionArgumentVariables);
            ExecuteNodes(currentFunction.SubNodes.ToList());
            var returnedVariable = CurrentContextScopeVariables.Where(variable => variable.Name == "$return").ToList()[0];
            FunctionScopeVariables.Pop();
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
            else if (expressionNode is ListInitializerExpressionNode)
            {
                var listInitializer = expressionNode as ListInitializerExpressionNode;
                return listInitializer.Elements.Select(listElementExpression=>EvaluateExpression(listElementExpression)).ToList(); //Return as List instead of IEnumerable because it's easier to access by index
            }
            else if (expressionNode is ListAccessorExpressionNode)
            {
                var listAccessor = expressionNode as ListAccessorExpressionNode;
                var currentVariable = GetReferencedVariable(listAccessor.ListVariableName);
                var accessorIndex = (int)Math.Truncate((double)EvaluateExpression(listAccessor.AccessorIndex)); //Turn the number into an integer for accessing
                return currentVariable.Value[accessorIndex];
            }
            else if (expressionNode is TableReferenceExpressionNode)
            {
                var tableReference = expressionNode as TableReferenceExpressionNode;
                var currentVariable = GetReferencedVariable(tableReference.TableQualifier.Name);
                dynamic intermediateMember = currentVariable.Value;
                foreach (var member in tableReference.TableQualifier.MemberChain)
                {
                    intermediateMember = intermediateMember[member];
                }
                return intermediateMember;
            }
            else if (expressionNode is FunctionCallExpressionNode)
            {
                var functionCallExpression = expressionNode as FunctionCallExpressionNode;
                var functionRef = GetReferencedFunction(functionCallExpression);
                var functionCallReturn = CreateFunctionCall(functionCallExpression, functionRef);
                return functionCallReturn.Value;
            }

            //Error evaluating expression (should really throw an error or something)
            return null;
        }

        private bool EvaluateToBoolean(dynamic value)
        {
            if (value == null)
                return false;

            if (value is bool)
                return value;

            if (value is string)
                return true;

            if (value is double)
                return value > 0;

            if (value is object)
                return true;

            return false;
        }

        private dynamic DoBinaryOperation(BinaryOperationNode expressionOperation)
        {
            //TODO: Implement all of them
            switch (expressionOperation.OperationType)
            {
                case ExpressionOperationType.Add:
                    return EvaluateExpression(expressionOperation.OperandA) + EvaluateExpression(expressionOperation.OperandB);

                case ExpressionOperationType.And:
                    return EvaluateToBoolean(EvaluateExpression(expressionOperation.OperandA)) && EvaluateToBoolean(EvaluateExpression(expressionOperation.OperandB));

                case ExpressionOperationType.Assignment:
                    var evaluatedValue = EvaluateExpression(expressionOperation.OperandB);
                    var targetVariableReferenceNode = expressionOperation.OperandA as VariableReferenceExpressionNode;
                    var targetVariable = GetReferencedVariable(targetVariableReferenceNode.VariableName);
                    targetVariable.Value = evaluatedValue;
                    break;

                case ExpressionOperationType.Divide:
                    return null;

                case ExpressionOperationType.Equals:
                    return EvaluateExpression(expressionOperation.OperandA) == EvaluateExpression(expressionOperation.OperandB) ? 1 : 0;

                case ExpressionOperationType.FunctionCall:
                    return null;

                case ExpressionOperationType.GreaterEquals:
                    return null;

                case ExpressionOperationType.GreaterThan:
                    return EvaluateExpression(expressionOperation.OperandA) == EvaluateExpression(expressionOperation.OperandB) ? 1 : 0;

                case ExpressionOperationType.LessEquals:
                    return null;

                case ExpressionOperationType.LessThan:
                    return null;

                case ExpressionOperationType.Modulo:
                    return null;

                case ExpressionOperationType.Multiply:
                    return EvaluateExpression(expressionOperation.OperandA) * EvaluateExpression(expressionOperation.OperandB);

                case ExpressionOperationType.Negate:
                    return null;

                case ExpressionOperationType.Not:
                    return null;

                case ExpressionOperationType.NotEquals:
                    return EvaluateExpression(expressionOperation.OperandA) != EvaluateExpression(expressionOperation.OperandB) ? 1 : 0;

                case ExpressionOperationType.OpenRoundBrace:
                    return null;

                case ExpressionOperationType.Or:
                    return EvaluateToBoolean(EvaluateExpression(expressionOperation.OperandA)) || EvaluateToBoolean(EvaluateExpression(expressionOperation.OperandB));

                case ExpressionOperationType.Subtract:
                    return null;

                case ExpressionOperationType.Xor:
                    return EvaluateToBoolean(EvaluateExpression(expressionOperation.OperandA)) ^ EvaluateToBoolean(EvaluateExpression(expressionOperation.OperandB));

                    //TODO: Implement the rest of them
            }
            //Error doing operation (should really throw an error or something)
            return null;
        }
    }
}
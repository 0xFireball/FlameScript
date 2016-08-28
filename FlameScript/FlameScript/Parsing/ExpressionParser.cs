﻿using FlameScript.Types;
using FlameScript.Types.Ast;
using FlameScript.Types.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace FlameScript.Parsing
{
    /// <summary>
    /// Parser for FlameScript expressions. Used internally by the parser.
    /// </summary>
    /// <remarks>
    /// Uses an algorithm based on the shunting-yard algorithm by Dijkstra.
    /// Good explanation here: http://wcipeg.com/wiki/Shunting_yard_algorithm
    /// </remarks>
    public class ExpressionParser
    {
        private Stack<ExpressionNode> working = new Stack<ExpressionNode>();
        private Stack<ExpressionOperationType> operators = new Stack<ExpressionOperationType>();
        private Stack<int> arity = new Stack<int>();
        private ExpressionParserGuessType guessType;

        //taken from http://en.wikipedia.org/wiki/Operators_in_C_and_C++
        private static readonly Dictionary<ExpressionOperationType, int> operationPrecedence = new Dictionary<ExpressionOperationType, int>()
        {
            {ExpressionOperationType.OpenRoundBrace, 0},
            { ExpressionOperationType.FunctionCall, 2 },
            { ExpressionOperationType.Negate, 3 },
            { ExpressionOperationType.Not, 3 },
            { ExpressionOperationType.Multiply, 5 },
            { ExpressionOperationType.Divide, 5 },
            { ExpressionOperationType.Modulo, 5 },
            { ExpressionOperationType.Add, 6 },
            { ExpressionOperationType.Subtract, 6 },
            { ExpressionOperationType.LessThan, 8 },
            { ExpressionOperationType.LessEquals, 8 },
            { ExpressionOperationType.GreaterThan, 8 },
            { ExpressionOperationType.GreaterEquals, 8 },
            { ExpressionOperationType.Equals, 9 },
            { ExpressionOperationType.NotEquals, 9 },
            { ExpressionOperationType.And, 10 },
            { ExpressionOperationType.Xor, 11 },
            { ExpressionOperationType.Or, 12 },
            { ExpressionOperationType.Assignment, 16 },
        };

        private static readonly ExpressionOperationType[] unaryOperators = { ExpressionOperationType.Negate, ExpressionOperationType.Not };

        private static readonly Dictionary<OperatorType, ExpressionOperationType> operatorToOperation = new Dictionary<OperatorType, ExpressionOperationType>()
        {
            { OperatorType.Add, ExpressionOperationType.Add},
            //{ OperatorType.SubstractNegate, /* not directly converitble, need to check for unary/binray */},
            { OperatorType.Multiply, ExpressionOperationType.Multiply},
            { OperatorType.Divide, ExpressionOperationType.Divide},
            { OperatorType.Modulo, ExpressionOperationType.Modulo},
            { OperatorType.Assignment,ExpressionOperationType.Assignment},
            { OperatorType.Equals, ExpressionOperationType.Equals},
            { OperatorType.GreaterThan, ExpressionOperationType.GreaterThan},
            { OperatorType.LessThan, ExpressionOperationType.LessThan},
            { OperatorType.GreaterEquals, ExpressionOperationType.GreaterEquals},
            { OperatorType.LessEquals, ExpressionOperationType.LessEquals},
            { OperatorType.NotEquals, ExpressionOperationType.NotEquals},
            { OperatorType.Not, ExpressionOperationType.Not},
            { OperatorType.And, ExpressionOperationType.And},
            { OperatorType.Or, ExpressionOperationType.Or},
            { OperatorType.Xor, ExpressionOperationType.Xor},
        };

        //used to distigush between unary and binary minus
        private bool lastTokenWasOperatorOrLeftBrace = true; //beginning of input is like a left brace

        /// <summary>
        /// Parses the given tokens to an AST.
        /// </summary>
        public ExpressionNode Parse(IEnumerable<Token> tokens)
        {
            bool sequenceWasEmpty = true;

            foreach (var token in tokens)
            {
                sequenceWasEmpty = false;
                if (token is LiteralToken)
                {
                    if (token is NumberLiteralToken)
                        working.Push(new NumberLiteralNode(((NumberLiteralToken)token).Number));
                    else if (token is StringLiteralToken)
                        working.Push(new StringLiteralNode(((StringLiteralToken)token).String));
                    lastTokenWasOperatorOrLeftBrace = false;
                }
                else if (token is OperatorToken)
                {
                    ExpressionOperationType op;
                    if (((OperatorToken)token).OperatorType == OperatorType.SubstractNegate) //need to check if binary or unary
                        op = lastTokenWasOperatorOrLeftBrace ? ExpressionOperationType.Negate : ExpressionOperationType.Subtract;
                    else //normal operator
                        op = operatorToOperation[((OperatorToken)token).OperatorType];

                    //TODO: Do we need to check for assosiativity? Only unary operators and assignments are rtl-assosiativ
                    ExpressionOperationType currentOperator;
                    while (operators.Count != 0 && operationPrecedence[currentOperator = operators.Peek()] > operationPrecedence[op]) //stack empty or only low precendence operators on stack
                    {
                        PopOperator();
                    }
                    operators.Push(op);
                    lastTokenWasOperatorOrLeftBrace = true;
                }
                else if (token is OpenBraceToken)
                {
                    var openBraceToken = token as OpenBraceToken;
                    if (openBraceToken.BraceType == BraceType.Round)
                    {
                        if (working.Count > 0 && working.Peek() is VariableReferenceExpressionNode)
                        {
                            //we have a "variable" sitting on top of the op stack, this must be the name of a function to be called.
                            //Create a function call instead
                            var variable = (VariableReferenceExpressionNode)working.Pop();
                            working.Push(new FunctionCallExpressionNode(variable.VariableName));
                            operators.Push(ExpressionOperationType.FunctionCall);
                            arity.Push(0);
                        }
                        operators.Push(ExpressionOperationType.OpenRoundBrace);
                    }
                    else if (openBraceToken.BraceType == BraceType.Square)
                    {
                        //Something to do with lists.
                        if (working.Count > 0 && working.Peek() is VariableReferenceExpressionNode)
                        {
                            //There's an identifier right before, it must be an accessor myList[0]
                            guessType = ExpressionParserGuessType.ListAccessor;
                        }
                        else
                        {
                            //Looks like a list literal: [1, 2, 3]
                            arity.Push(0); //This will be the list size
                            guessType = ExpressionParserGuessType.ListInitializer;
                        }
                    }

                    lastTokenWasOperatorOrLeftBrace = true;
                }
                else if (token is CloseBraceToken)
                {
                    var closeBraceToken = token as CloseBraceToken;
                    if (closeBraceToken.BraceType == BraceType.Round)
                    {
                        while (operators.Peek() != ExpressionOperationType.OpenRoundBrace)
                            PopOperator();
                        operators.Pop(); //pop the opening brace from the stack

                        if (operators.Count > 0 && operators.Peek() == ExpressionOperationType.FunctionCall) //function call sitting on top of the stack
                            PopOperator();
                    }
                    else if (closeBraceToken.BraceType == BraceType.Square)
                    {
                        //End of a list initializer or accessor

                        if (guessType == ExpressionParserGuessType.ListInitializer)
                        {
                            //Try building a list and return it

                            var listElements = new List<ExpressionNode>();
                            while (working.Count > 0) //Pop all the working expressions onto the list
                            {
                                var listElement = working.Pop();
                                listElements.Add(listElement);
                            }
                            listElements.Reverse(); //Compensate for the order of the stack
                            working.Push(new ListInitializerExpressionNode(listElements));
                        }
                        else if (guessType == ExpressionParserGuessType.ListAccessor)
                        {
                            var accessorIndex = working.Pop(); //This is the index of the list we are accessing; for example, in myList[2], the index is 2
                            var listVariableReference = working.Pop() as VariableReferenceExpressionNode; //This must be an identifier, currently parsed as variable reference
                            working.Push(new ListAccessorExpressionNode(listVariableReference.VariableName, accessorIndex));
                        }
                    }

                    lastTokenWasOperatorOrLeftBrace = false;
                }
                else if (token is IdentifierToken)
                {
                    if (token is TableIdentifierToken) //These tokens need special treatment
                    {
                        var tokenList = tokens.ToList();
                        var tokensToSkip = tokenList.IndexOf(token) + 1;
                        var upcomingTokens = new Stack<Token>(tokenList.Skip(tokensToSkip).Take(tokenList.Count - tokensToSkip).Reverse());
                        Token currentTestToken;
                        List<MemberAccessToken> memberAccessTokens = new List<MemberAccessToken>();
                        while ((currentTestToken = upcomingTokens.Pop()) is MemberAccessToken) //They can be stacked
                        {
                            memberAccessTokens.Add(currentTestToken as MemberAccessToken);
                            if (upcomingTokens.Count == 0)
                                break; //We're done.
                        }
                        working.Push(new TableReferenceExpressionNode(TableQualifier.Create(token as IdentifierToken, memberAccessTokens)));
                    }
                    else
                    {
                        //this could be a function call, but we would need a lookahead to check for an opening brace.
                        //just handle this as a variable reference and change it to a function when a open brace is found
                        working.Push(new VariableReferenceExpressionNode(((IdentifierToken)token).Content));
                    }
                }
                else if (token is MemberAccessToken)
                {
                    //Basically ignore these for now, they were handled by the tableidentifiertoken conditional
                }
                else if (token is ArgSeperatorToken)
                {
                    if (arity.Peek() == 0) //If it was previously assumed that there were no args
                    {
                        arity.Pop(); //There are at least
                        arity.Push(1); //One argument (at least)
                    }
                    arity.Push(arity.Pop() + 1); //increase arity on top of the stack

                    if (operators.Count > 0)
                    {
                        //It must be a function call, because there's a function name
                        while (operators.Peek() != ExpressionOperationType.OpenRoundBrace) //pop till the open brace of this function call
                            PopOperator();
                    }
                    else
                    {
                        //it's basically commas with no function call, AKA elements of a list
                    }
                }
                else
                    throw new UnexpectedTokenException("Found unknown token while parsing expression!", token);
            }

            if (sequenceWasEmpty)
                return null;

            //end of tokens, apply all the remaining operators
            while (operators.Count != 0)
                PopOperator();

            if (working.Count != 1)
                throw new ParsingException("Expression seems to be incomplete/invalid.");

            return working.Pop();
        }

        //pop and "apply" operator
        private void PopOperator()
        {
            var op = operators.Pop();
            if (op == ExpressionOperationType.FunctionCall)
            {
                //collect the args
                List<ExpressionNode> args = new List<ExpressionNode>();
                int functionArity = arity.Pop();
                for (int i = 0; i < functionArity; i++)
                    args.Add(working.Pop());
                args.Reverse(); //Reverse the args to get back the order they were put on the stack
                //add them to the function call sitting on top of the stack
                ((FunctionCallExpressionNode)working.Peek()).AddArguments(args);
            }
            else if (unaryOperators.Contains(op)) //Unary
                working.Push(new UnaryOperationNode(op, working.Pop()));
            else //binary
            {
                //reverse order of operands!
                var operandB = working.Pop();
                var operandA = working.Pop();

                working.Push(new BinaryOperationNode(op, operandA, operandB));
            }
        }
    }
}
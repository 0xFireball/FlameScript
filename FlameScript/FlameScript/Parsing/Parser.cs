using FlameScript.Types;
using FlameScript.Types.Ast;
using FlameScript.Types.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlameScript.Parsing
{
    // A parser for FlameScript
    public class Parser
    {
        public Token[] Tokens { get; }

        private int _readingPosition;
        private Stack<StatementSequenceNode> _scopes;

        private static readonly KeywordType[] typeKeywords = { KeywordType.Number, KeywordType.Void, KeywordType.String };

        public Parser(Token[] tokens)
        {
            this.Tokens = tokens;

            _readingPosition = 0;
            _scopes = new Stack<StatementSequenceNode>();
        }

        /// <summary>
        /// Returns a CodePosition with the position of the token in the code.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public CodePosition GetTokenPosition(Token token)
        {
            return new CodePosition();
        }

        public ProgramNode ParseToAst()
        {
            _scopes.Push(new ProgramNode());

            while (!EndOfProgram)
            {
                var upcomingToken = PeekNextToken();
                if (upcomingToken is KeywordToken)
                {
                    var keyword = (KeywordToken)NextToken();

                    if (_scopes.Count == 1) //we are a top level, the only valid keywords are variable types, starting a variable or function definition
                    {
                        if (keyword.IsTypeKeyword)
                        {
                            var varType = keyword.ToVariableType();
                            //it must be followed by a identifier:
                            var name = ReadNextToken<IdentifierToken>();
                            //so see what it is (function or variable):
                            Token lookahead = PeekNextToken();
                            if (lookahead is OperatorToken && (((OperatorToken)lookahead).OperatorType == OperatorType.Assignment) || lookahead is StatementSeparatorToken) //variable declaration
                            {
                                if (lookahead is OperatorToken)
                                    NextToken(); //skip the "="
                                _scopes.Peek().AddStatement(new VariableDeclarationNode(varType, name.Content, ExpressionNode.CreateFromTokens(ReadUntilStatementSeparator())));
                            }
                            else if (lookahead is OpenBraceToken && (((OpenBraceToken)lookahead).BraceType == BraceType.Round)) //function definition
                            {
                                var func = new FunctionDeclarationNode(name.Content);
                                _scopes.Peek().AddStatement(func); //add the function to the old (root) scope...
                                _scopes.Push(func); //...and set it a the new scope!
                                //Read the argument list
                                NextToken(); //skip the opening brace
                                while (!(PeekNextToken() is CloseBraceToken && ((CloseBraceToken)PeekNextToken()).BraceType == BraceType.Round)) //TODO: Refactor using readUntilClosingBrace?
                                {
                                    var argType = ReadNextToken<KeywordToken>();
                                    if (!argType.IsTypeKeyword)
                                        throw new ParsingException("Expected type keyword!");
                                    var argName = ReadNextToken<IdentifierToken>();
                                    func.AddParameter(new ParameterDeclarationNode(argType.ToVariableType(), argName.Content));
                                    if (PeekNextToken() is ArgSeperatorToken) //TODO: Does this allow (int a int b)-style functions? (No arg-seperator!)
                                        NextToken(); //skip the sperator
                                }
                                NextToken(); //skip the closing brace
                                var curlyBrace = ReadNextToken<OpenBraceToken>();
                                if (curlyBrace.BraceType != BraceType.Curly)
                                    throw new ParsingException("Wrong brace type found!");
                            }
                            else
                                throw new Exception("The parser encountered an unexpected token.");
                        }
                        else
                            throw new ParsingException("Found non-type keyword on top level.");
                    }
                    else //we are in a nested scope
                    {
                        //TODO: Can we avoid the code duplication from above?
                        if (keyword.IsTypeKeyword) //local variable declaration!
                        {
                            var varType = keyword.ToVariableType();
                            //it must be followed by a identifier:
                            var name = ReadNextToken<IdentifierToken>();
                            //so see what it is (function or variable):
                            Token lookahead = PeekNextToken();
                            if (lookahead is OperatorToken && (((OperatorToken)lookahead).OperatorType == OperatorType.Assignment) || lookahead is StatementSeparatorToken) //variable declaration
                            {
                                if (lookahead is OperatorToken)
                                    NextToken(); //skip the "="
                                _scopes.Peek().AddStatement(new VariableDeclarationNode(varType, name.Content, ExpressionNode.CreateFromTokens(ReadUntilStatementSeparator())));
                            }
                        }
                        else
                        {
                            switch (keyword.KeywordType)
                            {
                                case KeywordType.Return:
                                    _scopes.Peek().AddStatement(new ReturnStatementNode(ExpressionNode.CreateFromTokens(ReadUntilStatementSeparator())));
                                    break;

                                case KeywordType.If:
                                    var @if = new IfStatementNode(ExpressionNode.CreateFromTokens(ReadUntilClosingBrace()));
                                    _scopes.Peek().AddStatement(@if);
                                    _scopes.Push(@if);
                                    break;

                                case KeywordType.While:
                                    var @while = new WhileLoopNode(ExpressionNode.CreateFromTokens(ReadUntilClosingBrace()));
                                    _scopes.Peek().AddStatement(@while);
                                    _scopes.Push(@while);
                                    break;

                                default:
                                    throw new ParsingException("Unexpected keyword type.");
                            }
                        }
                    }
                }
                else if (upcomingToken is IdentifierToken && _scopes.Count > 1) //in nested scope
                {
                    var name = ReadNextToken<IdentifierToken>();
                    if (upcomingToken is OperatorToken && ((OperatorToken)upcomingToken).OperatorType == OperatorType.Assignment) //variable assignment
                    {
                        NextToken(); //skip the "="
                        _scopes.Peek().AddStatement(new VariableAssignmentNode(name.Content, ExpressionNode.CreateFromTokens(ReadUntilStatementSeparator())));
                    }
                    else //lone expression (incl. function calls!)
                        _scopes.Peek().AddStatement(ExpressionNode.CreateFromTokens(new[] { name }.Concat(ReadUntilStatementSeparator()))); //don't forget the name here!
                }
                else if (upcomingToken is CloseBraceToken)
                {
                    var brace = ReadNextToken<CloseBraceToken>();
                    if (brace.BraceType != BraceType.Curly)
                        throw new ParsingException("Wrong brace type found!");
                    _scopes.Pop(); //Scope has been closed!
                }
                else
                    throw new UnexpectedTokenException("The parser ran into an unexpeted token", upcomingToken);
            }

            if (_scopes.Count != 1)
                throw new ParsingException("The _scopes are not correctly nested.");

            return (ProgramNode)_scopes.Pop();
        }

        private IEnumerable<Token> ReadTokenSequence(params Type[] expectedTypes)
        {
            foreach (var t in expectedTypes)
            {
                var upcomingToken = PeekNextToken();
                if (!t.IsAssignableFrom(upcomingToken.GetType()))
                    throw new UnexpectedTokenException("Unexpected token", upcomingToken);
                yield return NextToken();
            }
        }

        private IEnumerable<Token> ReadUntilClosingBrace()
        {
            //TODO: Only allow round braces, handle nested braces!
            while (!EndOfProgram && !(PeekNextToken() is CloseBraceToken))
            {
                yield return NextToken();
            }
            NextToken(); //skip the closing brace
        }

        private IEnumerable<Token> ReadUntilStatementSeparator()
        {
            var upcomingToken = PeekNextToken();
            while (!EndOfProgram && !(upcomingToken is StatementSeparatorToken))
            {
                yield return NextToken();
                upcomingToken = PeekNextToken(); //Update upcomingToken
            }
            NextToken(); //Skip the statement separator
        }

        private TExpected ReadNextToken<TExpected>() where TExpected : Token
        {
            var upcomingToken = PeekNextToken();
            if (upcomingToken is TExpected)
            {
                //Valid, advance pointer and return
                return (TExpected)NextToken();
            }
            else
            {
                throw new UnexpectedTokenException("Unexpected token", upcomingToken);
            }
        }

        private Token NextToken()
        {
            var ret = PeekNextToken();
            _readingPosition++;
            return ret;
        }

        private Token PeekNextToken()
        {
            return Tokens[_readingPosition];
        }

        private bool EndOfProgram => _readingPosition >= Tokens.Length;
    }
}
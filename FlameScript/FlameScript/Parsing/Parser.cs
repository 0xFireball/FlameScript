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

        /// <summary>
        /// A quick forwarder to _scopes.Peek()
        /// </summary>
        private StatementSequenceNode _currentScope => _scopes.Peek();

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
                    var globalScope = _scopes.Count == 1;
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
                            _currentScope.AddStatement(new VariableDeclarationNode(varType, name.Content, ExpressionNode.CreateFromTokens(ReadUntilStatementSeparator())));
                        }
                        //If in the global scope, it can also be a function declaration
                        else if (globalScope && lookahead is OpenBraceToken && (((OpenBraceToken)lookahead).BraceType == BraceType.Round)) //function definition
                        {
                            var func = new FunctionDeclarationNode(name.Content);
                            _currentScope.AddStatement(func); //add the function to the old (root) scope...
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
                        //We can't have anything other than what's listed above on the global scope
                    }
                    //Not a type keyword. If on a local scope, it may be other keywords
                    else if (!globalScope)
                    {
                        switch (keyword.KeywordType)
                        {
                            case KeywordType.Return:
                                _currentScope.AddStatement(new ReturnStatementNode(ExpressionNode.CreateFromTokens(ReadUntilStatementSeparator())));
                                break;

                            case KeywordType.If:
                                var @if = new IfStatementNode(ExpressionNode.CreateFromTokens(ReadUntilClosingBrace(true)));
                                _currentScope.AddStatement(@if); //Add if statement to previous scope
                                _scopes.Push(@if); //...and set it a the new scope!
                                NextToken(); //skip the opening curly brace
                                break;

                            case KeywordType.While:
                                var @while = new WhileLoopNode(ExpressionNode.CreateFromTokens(ReadUntilClosingBrace(true)));
                                _currentScope.AddStatement(@while);
                                _scopes.Push(@while);
                                NextToken(); //skip the opening curly brace
                                break;

                            default:
                                throw new ParsingException("Unexpected keyword type.");
                        }
                    }
                    else //It was not a keyword, and it was a global scope
                        throw new ParsingException("Found non-type keyword on global scope.");
                }
                else if (upcomingToken is IdentifierToken && _scopes.Count > 1) //in a nested scope
                {
                    var identifierToken = upcomingToken as IdentifierToken;
                    NextToken(); //Step past the identifier token
                    var nextToken = PeekNextToken(); //Read the next token
                    if (nextToken is OperatorToken && ((OperatorToken)nextToken).OperatorType == OperatorType.Assignment) //variable assignment
                    {
                        NextToken(); //skip the "="
                        _currentScope.AddStatement(new VariableAssignmentNode(identifierToken.Content, ExpressionNode.CreateFromTokens(ReadUntilStatementSeparator())));
                    }
                    else if (nextToken is MemberAccessToken)
                    {
                        List<MemberAccessToken> memberAccessTokens = new List<MemberAccessToken>();
                        Token currentTestToken;
                        while ((currentTestToken = PeekNextToken()) is MemberAccessToken) //They can be stacked
                        {
                            NextToken(); //Advance
                            memberAccessTokens.Add(currentTestToken as MemberAccessToken);
                        }
                        if (currentTestToken is OperatorToken && ((OperatorToken)currentTestToken).OperatorType == OperatorType.Assignment) //table member assignment
                        {
                            NextToken(); //skip the "="
                                         //Tokens until statement end have to be preloaded as a 'temporary workaround' to allow looking forward
                            var expressionTokens = ReadUntilStatementSeparator().ToList();
                            _currentScope.AddStatement(new TableAssignmentNode(TableQualifier.Create(identifierToken, memberAccessTokens), ExpressionNode.CreateFromTokens(expressionTokens)));
                        }
                    }
                    else //lone expression (incl. function calls!)
                        _currentScope.AddStatement(ExpressionNode.CreateFromTokens(new[] { identifierToken }.Concat(ReadUntilStatementSeparator()))); //don't forget the name here!
                }
                else if (upcomingToken is CloseBraceToken)
                {
                    //Closing a scope
                    var brace = ReadNextToken<CloseBraceToken>();
                    if (brace.BraceType != BraceType.Curly)
                        throw new ParsingException("Wrong brace type found!");
                    _scopes.Pop(); //Scope has been closed!
                }
                else
                    throw new UnexpectedTokenException("The parser ran into an unexpected token", upcomingToken);
            }

            if (_scopes.Count != 1)
                throw new ParsingException("The scopes are not correctly nested.");

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

        private IEnumerable<Token> ReadUntilClosingBrace(bool includeClosingBrace = false)
        {
            //TODO: Only allow round braces, handle nested braces!
            while (!EndOfProgram && !(PeekNextToken() is CloseBraceToken))
            {
                yield return NextToken();
            }
            if (includeClosingBrace)
                yield return PeekNextToken(); //Include the closing brace
            NextToken(); //advance after the closing brace
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
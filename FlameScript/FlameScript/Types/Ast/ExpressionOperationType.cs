namespace FlameScript.Types.Ast
{
    // we need the enum additionally to OperatorType to support the parser,
    // which must decide between unary and binary minus and support function
    // calls, which are no "operators" in the token's sense.
    public enum ExpressionOperationType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        Assignment,
        Equals,
        GreaterThan,
        LessThan,
        GreaterEquals,
        LessEquals,
        NotEquals,
        Not,
        And,
        Or,
        Negate,
        FunctionCall,
        OpenBrace,
        Xor,
    }
}
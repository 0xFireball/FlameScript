using System;
using System.Collections.Generic;

namespace FlameScript.Types.Tokens
{
    public class OperatorToken : Token
    {
        private static readonly Dictionary<string, OperatorType> validOperators = new Dictionary<string, OperatorType>()
        {
            { "+", OperatorType.Add },
            { "&&", OperatorType.And },
            { "=", OperatorType.Assignment },
            { "/", OperatorType.Divide },
            { "==", OperatorType.Equals },
            { ">=", OperatorType.GreaterEquals },
            { ">", OperatorType.GreaterThan },
            { "<=", OperatorType.LessEquals },
            { "<", OperatorType.LessThan },
            { "%", OperatorType.Modulo },
            { "*", OperatorType.Multiply },
            { "!", OperatorType.Not },
            { "!=", OperatorType.NotEquals },
            { "||", OperatorType.Or },
            { "-", OperatorType.SubstractNegate },
        };

        public OperatorType OperatorType { get; private set; }

        public OperatorToken(string content)
            : base(content)
        {
            if (!validOperators.ContainsKey(content))
                throw new ArgumentException("The content is not a valid operator.", nameof(content));

            OperatorType = validOperators[content];
        }
    }
}
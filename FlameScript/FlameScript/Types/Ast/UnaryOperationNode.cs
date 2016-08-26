using System;
using System.Linq;

namespace FlameScript.Types.Ast
{
    public class UnaryOperationNode : ExpressionNode
    {
        public ExpressionOperationType OperationType { get; private set; }
        public ExpressionNode Operand { get; private set; }

        private static readonly ExpressionOperationType[] validOperators = { ExpressionOperationType.Negate, ExpressionOperationType.Not };

        public UnaryOperationNode(ExpressionOperationType opType, ExpressionNode operand)
        {
            if (!validOperators.Contains(opType))
                throw new ArgumentException("Invalid unary operator given!", nameof(opType));

            OperationType = opType;
            Operand = operand;
        }
    }
}
namespace FlameScript.Types.Ast
{
    public class NumberLiteralNode : ExpressionNode
    {
        public double Value { get; private set; }

        public NumberLiteralNode(double value)
        {
            Value = value;
        }
    }
}
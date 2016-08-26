namespace FlameScript.Types.Ast
{
    public class NumberLiteralNode : LiteralNode
    {
        public double Value { get; private set; }

        public NumberLiteralNode(double value)
        {
            Value = value;
        }
    }
}
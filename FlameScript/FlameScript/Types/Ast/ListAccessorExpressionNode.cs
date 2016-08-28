namespace FlameScript.Types.Ast
{
    public class ListAccessorExpressionNode : ExpressionNode
    {
        public ExpressionNode AccessorIndex { get; }
        public string ListVariableName { get; }

        public ListAccessorExpressionNode(string listVariableName, ExpressionNode accessorIndex)
        {
            ListVariableName = listVariableName;
            AccessorIndex = accessorIndex;
        }
    }
}
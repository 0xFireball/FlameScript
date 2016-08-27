namespace FlameScript.Types.Ast
{
    public class TableReferenceExpressionNode : ExpressionNode
    {
        public TableQualifier TableQualifier { get; private set; }

        public TableReferenceExpressionNode(TableQualifier tableQualifier)
        {
            TableQualifier = tableQualifier;
        }
    }
}
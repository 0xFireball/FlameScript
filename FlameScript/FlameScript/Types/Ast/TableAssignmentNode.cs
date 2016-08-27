using FlameScript.Parsing;

namespace FlameScript.Types.Ast
{
    public class TableAssignmentNode : AstNode
    {
        public TableQualifier TableQualifier { get; private set; }
        public ExpressionNode ValueExpression { get; private set; }

        public TableAssignmentNode(TableQualifier tableQualifier, ExpressionNode expr)
        {
            if (expr == null)
                throw new ParsingException("The assigned expression may not be null!");

            TableQualifier = tableQualifier;
            ValueExpression = expr;
        }
    }
}
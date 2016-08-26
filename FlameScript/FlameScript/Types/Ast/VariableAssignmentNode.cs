using FlameScript.Parsing;

namespace FlameScript.Types.Ast
{
    public class VariableAssignmentNode : AstNode
    {
        public string VariableName { get; private set; }
        public ExpressionNode ValueExpression { get; private set; }

        public VariableAssignmentNode(string name, ExpressionNode expr)
        {
            if (expr == null)
                throw new ParsingException("The assinged expression may not be null!");

            VariableName = name;
            ValueExpression = expr;
        }
    }
}
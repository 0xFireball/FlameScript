namespace FlameScript.Types.Ast
{
    public class VariableReferenceExpressionNode : ExpressionNode
    {
        public string VariableName { get; private set; }

        public VariableReferenceExpressionNode(string varName)
        {
            VariableName = varName;
        }
    }
}
using System.Collections.Generic;

namespace FlameScript.Types.Ast
{
    public class TableMemberInvocationNode : AstNode
    {
        public TableQualifier TableQualifier { get; private set; }
        public IEnumerable<ExpressionNode> Arguments { get; }

        public TableMemberInvocationNode(TableQualifier tableQualifier, IEnumerable<ExpressionNode> arguments)
        {
            TableQualifier = tableQualifier;
            Arguments = arguments;
        }
    }
}
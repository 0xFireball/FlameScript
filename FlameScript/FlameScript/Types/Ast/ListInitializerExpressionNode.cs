using System.Collections.Generic;

namespace FlameScript.Types.Ast
{
    public class ListInitializerExpressionNode : ExpressionNode
    {
        public IEnumerable<ExpressionNode> Elements { get; }

        public ListInitializerExpressionNode(IEnumerable<ExpressionNode> listElements)
        {
            Elements = listElements;
        }
    }
}
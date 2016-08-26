using FlameScript.Parsing;

namespace FlameScript.Types.Ast
{
    public class WhileLoopNode : LoopStatementNode
    {
        public ExpressionNode Condition { get; private set; }

        public WhileLoopNode(ExpressionNode cond)
        {
            if (cond == null)
                throw new ParsingException("Parser: An while loop must conatain a condition!");
        }
    }
}
using System.Collections.Generic;

namespace FlameScript.Types.Ast
{
    public class FunctionCallExpressionNode : ExpressionNode
    {
        public string FunctionName { get; private set; }

        public IEnumerable<ExpressionNode> Arguments
        {
            get
            {
                return _arguments;
            }
        }

        public int ArgumentCount
        {
            get { return _arguments.Count; }
        }

        private List<ExpressionNode> _arguments;

        public FunctionCallExpressionNode(string functionName)
        {
            FunctionName = functionName;

            _arguments = new List<ExpressionNode>();
        }

        public FunctionCallExpressionNode(string functionName, params ExpressionNode[] args)
            : this(functionName)
        {
            AddArguments(args);
        }

        public void AddArgument(ExpressionNode arg)
        {
            _arguments.Add(arg);
        }

        public void AddArguments(IEnumerable<ExpressionNode> args)
        {
            _arguments.AddRange(args);
        }
    }
}
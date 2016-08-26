using System.Collections.Generic;

namespace FlameScript.Types.Ast
{
    public class FunctionDeclarationNode : StatementSequenceNode
    {
        public string FunctionName { get; private set; }

        public IEnumerable<ParameterDeclarationNode> Parameters
        {
            get
            {
                return paramters;
            }
        }

        private List<ParameterDeclarationNode> paramters;

        public FunctionDeclarationNode(string name)
        {
            FunctionName = name;

            paramters = new List<ParameterDeclarationNode>();
        }

        public void AddParameter(ParameterDeclarationNode param)
        {
            paramters.Add(param);
        }
    }
}
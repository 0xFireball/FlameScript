using System;
using System.Collections.Generic;

namespace FlameScript.Types.Ast
{
    public class FunctionDeclarationNode : StatementSequenceNode
    {
        public string FunctionName { get; private set; }

        public FunctionBodyType BodyType { get; set; } = FunctionBodyType.FlameScript;

        public Delegate NativeBody { get; set; }

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
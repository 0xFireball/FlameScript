using FlameScript.Types.Tokens;

namespace FlameScript.Types.Ast
{
    public class ParameterDeclarationNode
    {
        public VariableType Type { get; private set; }
        public string Name { get; private set; }

        public ParameterDeclarationNode(VariableType type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}
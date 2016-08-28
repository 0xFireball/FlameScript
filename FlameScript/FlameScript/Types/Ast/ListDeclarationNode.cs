using FlameScript.Types.Tokens;

namespace FlameScript.Types.Ast
{
    public class ListDeclarationNode : VariableDeclarationNode
    {
        /// <summary>
        /// Creates a new instance of the ListDeclarationNode class.
        /// </summary>
        /// <param name="listType">The type of the elements of the list.</param>
        /// <param name="name">The name of the list variable.</param>
        /// <param name="initialValue">A expression used to initialise the list variable initially or null to use the default value.</param>
        public ListDeclarationNode(VariableType listType, string name, ExpressionNode initialValue) : base(listType, name, initialValue)
        {
        }
    }
}
using FlameScript.Types.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace FlameScript.Types.Ast
{
    public class TableQualifier
    {
        /// <summary>
        /// The identifier name of the table
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A chain of accessing members. For example, the table a.b.c 's qualifier would have the member chain as {"a","b","c"}
        /// </summary>
        public IEnumerable<string> MemberChain { get; }

        public TableQualifier(string name, IEnumerable<string> memberChain)
        {
            Name = name;
            MemberChain = memberChain;
        }

        internal static TableQualifier Create(IdentifierToken identifier, List<MemberAccessToken> memberAccessors)
        {
            return new TableQualifier(identifier.Content, memberAccessors.Select(accessor => accessor.MemberName));
        }
    }
}
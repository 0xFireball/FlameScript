using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameScript.Types.Ast
{
    public class StringLiteralNode : LiteralNode
    {
        public string Value;

        public StringLiteralNode(string value)
        {
            Value = value;
        }
    }
}

using System.Collections.Generic;

namespace FlameScript.Compiler.AstEmitters
{
    public abstract class StatementSequenceEmitter : AstEmitter
    {
        public List<StatementSequenceEmitter> SubNodes { get; } = new List<StatementSequenceEmitter>();

        public virtual void EmitCode(HappyPenguinCodeEmitter emitter)
        {
            EmitCodeOnAllSubnodes(this, emitter);
        }

        private void EmitCodeOnAllSubnodes(StatementSequenceEmitter node, HappyPenguinCodeEmitter emitter)
        {
            foreach (var subNode in node.SubNodes)
            {
                subNode.EmitCode(emitter);
                if (subNode.SubNodes.Count > 0) //If there are subNodes, recurse again
                    EmitCodeOnAllSubnodes(subNode, emitter);
            }
        }
    }
}
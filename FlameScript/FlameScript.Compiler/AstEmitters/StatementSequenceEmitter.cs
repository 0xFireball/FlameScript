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

        private void EmitCodeOnAllSubnodes(StatementSequenceEmitter subNode, HappyPenguinCodeEmitter emitter)
        {
            subNode.EmitCode(emitter);

            foreach (var subSubNode in subNode.SubNodes)
            {
                if (subSubNode.SubNodes.Count > 0) //If there are subNodes, recurse again
                    EmitCodeOnAllSubnodes(subSubNode, emitter);
            }
        }
    }
}
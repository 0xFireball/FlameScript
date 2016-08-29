using System.Collections.Generic;

namespace HappyPenguinVM.Types
{
    /// <summary>
    /// A class representing a HappyPenguinVM Program, or a list of HappyPenguinVM instructions
    /// </summary>
    public class HappyPenguinVMProgram : List<CodeInstruction>
    {
        public HappyPenguinVMProgram()
        {
        }

        public HappyPenguinVMProgram(IEnumerable<CodeInstruction> collection) : base(collection)
        {
        }
    }
}
using HappyPenguinVM.Types;

namespace HappyPenguinVM.Execution
{
    public class ProgramExecutor
    {
        private const int DEFAULT_MEMORY_SIZE = 1024; //1K cells, or 4KiB memory

        private int[] memory; //one memory cell is sizeof(int) big

        private int programCounter;
        private int stackPointer;
        private int heapPointer;
        private int framePointer;
        private int extremePointer; //TODO: Needed?
        private HappyPenguinVMProgram code;


    }
}
using HappyPenguinVM.Types;
using System;

namespace HappyPenguinVM.Execution
{
    public class ProgramExecutor
    {
        public int[] Memory => memory;

        private const int DEFAULT_MEMORY_SIZE = 1024; //1K cells, or 4KiB memory

        private int[] memory; //one memory cell is sizeof(int) big
        private int _memorySize;

        private int programCounter; //Current instruction to execute
        private int stackPointer; //Position of stack

        //private int heapPointer; //Not used yet
        private int framePointer; //Points to start of stack frame

        private HappyPenguinVMProgram code;

        public ProgramExecutor(HappyPenguinVMProgram code) : this(code, 0)
        {
        }

        public ProgramExecutor(HappyPenguinVMProgram code, int memorySize)
        {
            if (memorySize < 0)
                throw new ArgumentOutOfRangeException(nameof(memorySize), "Negative memory size given");

            if (memorySize > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(memorySize), "Memory size to big (addresses are only 16 bits!)");

            if (memorySize == 0) //auto
                memorySize = DEFAULT_MEMORY_SIZE;

            _memorySize = memorySize;

            this.code = code;
        }

        /// <summary>
        /// Initializes the machine.
        /// </summary>
        public void InitializeMachine()
        {
            memory = new int[_memorySize]; //Reset memory
            programCounter = 0;
            stackPointer = 0;
            //heapPointer = memory.Length - 1;
            framePointer = 0;
        }

        /// <summary>
        /// Begin code execution
        /// </summary>
        public void ExecuteCode()
        {
            InitializeMachine();

            CodeInstruction instruction = code[programCounter]; //The first line to execute

            while (instruction.OpCode != OpCode.Halt)
            {
                ExecuteInstruction(instruction);
                programCounter++;
                instruction = code[programCounter];
            }
        }

        private void ExecuteInstruction(CodeInstruction instruction)
        {
            //Generally, Not much operand checking is done here (as in a real hardware)
            //if some args are wrong, the behavior is not defined.
            //Don't mess up...

            switch (instruction.OpCode)
            {
                case OpCode.Call:
                    framePointer = stackPointer; //Start of current frame
                    int tmp = programCounter;
                    programCounter = memory[stackPointer];
                    memory[stackPointer] = tmp;
                    break;

                case OpCode.Return:
                    programCounter = memory[framePointer]; //Get old program counter from previous frame
                    stackPointer = framePointer - instruction.ByteArg1;
                    framePointer = memory[framePointer - 1];
                    break;

                case OpCode.Nop:
                    //do nothing...
                    break;

                case OpCode.Halt:
                    //do nothing, will be handled by main excecution cycle
                    break;

                default:
                    throw new InvalidOpCodeException("An unknown opcode was encountered: ", instruction.OpCode);
            }
        }
    }
}
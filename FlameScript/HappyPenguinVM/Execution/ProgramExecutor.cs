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

        private int programCounter;
        private int stackPointer;
        private int heapPointer;
        private int framePointer;
        private int extremePointer; //TODO: Needed?
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
            heapPointer = memory.Length - 1;
            framePointer = 0;
            extremePointer = 0;
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
                case OpCode.LoadC:
                    stackPointer++;
                    memory[stackPointer] = instruction.UShortArg;
                    break;

                case OpCode.Load:
                    for (int i = instruction.ByteArg1 - 1; i >= 0; i--)
                        memory[stackPointer + i] = memory[memory[stackPointer] + i];
                    stackPointer += instruction.ByteArg1 - 1;
                    break;

                case OpCode.LoadA:
                    stackPointer++;
                    for (int i = instruction.ByteArg2 - 1; i >= 0; i--)
                        memory[stackPointer + i] = memory[instruction.ByteArg1 + i];
                    stackPointer += instruction.ByteArg2 - 1;
                    break;

                case OpCode.Dup:
                    memory[stackPointer + 1] = memory[stackPointer];
                    stackPointer++;
                    break;

                case OpCode.LoadRc:
                    stackPointer++;
                    memory[stackPointer] = framePointer + instruction.UShortArg;
                    break;

                case OpCode.LoadR:
                    stackPointer++;
                    for (int i = instruction.ByteArg2 - 1; i >= 0; i--)
                        memory[stackPointer + i] = memory[framePointer + instruction.ByteArg1 + i];
                    stackPointer += instruction.ByteArg2 - 1;
                    break;

                case OpCode.LoadMc:
                    stackPointer++;
                    memory[stackPointer] = memory[framePointer - 3] + instruction.ByteArg1;
                    break;

                case OpCode.LoadM:
                    stackPointer++;
                    for (int i = instruction.ByteArg2 - 1; i >= 0; i--)
                        memory[stackPointer + i] = memory[memory[framePointer - 3] + instruction.ByteArg1];
                    stackPointer += instruction.ByteArg2 - 1;
                    break;

                case OpCode.LoadV:
                    memory[stackPointer + 1] = memory[memory[memory[stackPointer - 2]] + instruction.ByteArg1];
                    stackPointer++;
                    break;

                case OpCode.LoadSc:
                    memory[stackPointer + 1] = stackPointer - instruction.ByteArg1;
                    stackPointer++;
                    break;

                case OpCode.LoadS:
                    stackPointer++;
                    memory[stackPointer] = memory[stackPointer - instruction.ByteArg1];
                    break;

                case OpCode.Pop:
                    stackPointer -= instruction.ByteArg1;
                    break;

                case OpCode.Push:
                    for (int i = 0; i < instruction.ByteArg1; i++)
                        memory[memory[stackPointer] + i] = memory[stackPointer - instruction.ByteArg1 + i];
                    stackPointer--;
                    break;

                case OpCode.PushA:
                    stackPointer++;
                    for (int i = 0; i < instruction.ByteArg2; i++)
                        memory[instruction.ByteArg1 + i] = memory[stackPointer - instruction.ByteArg2 + i];
                    stackPointer--;
                    break;

                case OpCode.PushR:
                    stackPointer++;
                    for (int i = 0; i < instruction.ByteArg2; i++)
                        memory[framePointer + instruction.ByteArg1 + i] = memory[stackPointer - instruction.ByteArg2 + i];
                    stackPointer--;
                    break;

                case OpCode.PushM:
                    stackPointer++;
                    for (int i = 0; i < instruction.ByteArg2; i++)
                        memory[memory[framePointer - 3] + instruction.ByteArg1 + i] = memory[stackPointer - instruction.ByteArg2 + i];
                    stackPointer--;
                    break;

                case OpCode.Jump:
                    programCounter = instruction.UShortArg;
                    break;

                case OpCode.JumpZ:
                    if (memory[stackPointer] == 0)
                        programCounter = instruction.UShortArg;
                    stackPointer--;
                    break;

                case OpCode.JumpI:
                    programCounter = instruction.UShortArg + memory[stackPointer];
                    stackPointer--;
                    break;

                case OpCode.Add:
                    memory[stackPointer - 1] = memory[stackPointer - 1] + memory[stackPointer];
                    stackPointer--;
                    break;

                case OpCode.Sub:
                    memory[stackPointer - 1] = memory[stackPointer - 1] - memory[stackPointer];
                    stackPointer--;
                    break;

                case OpCode.Mul:
                    memory[stackPointer - 1] = memory[stackPointer - 1] * memory[stackPointer];
                    stackPointer--;
                    break;

                case OpCode.Div:
                    memory[stackPointer - 1] = memory[stackPointer - 1] / memory[stackPointer];
                    stackPointer--;
                    break;

                case OpCode.Mod:
                    memory[stackPointer - 1] = memory[stackPointer - 1] % memory[stackPointer];
                    stackPointer--;
                    break;

                case OpCode.Neg:
                    memory[stackPointer] = -memory[stackPointer];
                    break;

                case OpCode.Eq:
                    memory[stackPointer - 1] = (memory[stackPointer - 1] == memory[stackPointer]) ? 1 : 0;
                    stackPointer--;
                    break;

                case OpCode.Neq:
                    memory[stackPointer - 1] = (memory[stackPointer - 1] != memory[stackPointer]) ? 1 : 0;
                    stackPointer--;
                    break;

                case OpCode.Le:
                    memory[stackPointer - 1] = (memory[stackPointer - 1] < memory[stackPointer]) ? 1 : 0;
                    stackPointer--;
                    break;

                case OpCode.Leq:
                    memory[stackPointer - 1] = (memory[stackPointer - 1] <= memory[stackPointer]) ? 1 : 0;
                    stackPointer--;
                    break;

                case OpCode.Gr:
                    memory[stackPointer - 1] = (memory[stackPointer - 1] > memory[stackPointer]) ? 1 : 0;
                    stackPointer--;
                    break;

                case OpCode.Geq:
                    memory[stackPointer - 1] = (memory[stackPointer - 1] >= memory[stackPointer]) ? 1 : 0;
                    stackPointer--;
                    break;

                case OpCode.And:
                    memory[stackPointer - 1] = (memory[stackPointer - 1] != 0 && memory[stackPointer] != 0) ? 1 : 0;
                    stackPointer--;
                    break;

                case OpCode.Or:
                    memory[stackPointer - 1] = (memory[stackPointer - 1] != 0 || memory[stackPointer] != 0) ? 1 : 0;
                    stackPointer--;
                    break;

                case OpCode.Not:
                    memory[stackPointer] = (memory[stackPointer] == 0) ? 1 : 0;
                    break;

                case OpCode.Mark:
                    memory[stackPointer + 1] = extremePointer;
                    memory[stackPointer + 2] = framePointer;
                    stackPointer += 2;
                    break;

                case OpCode.Call:
                    framePointer = stackPointer;
                    int tmp = programCounter;
                    programCounter = memory[stackPointer];
                    memory[stackPointer] = tmp;
                    break;

                case OpCode.Enter:
                    extremePointer = stackPointer + instruction.ByteArg1;
                    if (extremePointer >= heapPointer)
                        throw new StackOverflowException();
                    break;

                case OpCode.Alloc:
                    stackPointer += instruction.ByteArg1;
                    break;

                case OpCode.Slide:
                    if (instruction.ByteArg1 > 0)
                    {
                        if (instruction.ByteArg2 == 0)
                            stackPointer -= instruction.ByteArg1;
                        else
                        {
                            stackPointer -= instruction.ByteArg1 + instruction.ByteArg2;
                            for (int i = 0; i < instruction.ByteArg2; i++)
                            {
                                stackPointer++;
                                memory[stackPointer] = memory[stackPointer + instruction.ByteArg1];
                            }
                        }
                    }
                    break;

                case OpCode.Return:
                    programCounter = memory[framePointer];
                    extremePointer = memory[framePointer - 2];
                    if (extremePointer >= heapPointer)
                        throw new StackOverflowException();
                    stackPointer = framePointer - instruction.ByteArg1;
                    framePointer = memory[framePointer - 1];
                    break;

                case OpCode.New:
                    if (heapPointer - memory[stackPointer] > extremePointer)
                    {
                        heapPointer = heapPointer - memory[stackPointer];
                        memory[stackPointer] = heapPointer;
                    }
                    else
                        memory[stackPointer] = 0; //out of memory
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
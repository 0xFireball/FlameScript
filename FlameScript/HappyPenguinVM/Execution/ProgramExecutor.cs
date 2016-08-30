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
        private Registers registers;

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
            registers = new Registers();
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
                case OpCode.MovReg:
                    var regId = (RegisterId)instruction.ByteArg1;
                    var val = instruction.ByteArg2;

                    switch (regId)
                    {
                        case RegisterId.A:
                            registers.A = val;
                            break;

                        case RegisterId.B:
                            registers.B = val;
                            break;

                        case RegisterId.C:
                            registers.C = val;
                            break;

                        case RegisterId.D:
                            registers.D = val;
                            break;
                    }
                    break;

                case OpCode.LoadA:
                    ushort sourceAddress = instruction.UShortArg;
                    registers.A = (byte)memory[sourceAddress];
                    break;

                case OpCode.LoadB:
                    sourceAddress = instruction.UShortArg;
                    registers.B = (byte)memory[sourceAddress];
                    break;

                case OpCode.LoadC:
                    sourceAddress = instruction.UShortArg;
                    registers.C = (byte)memory[sourceAddress];
                    break;

                case OpCode.LoadD:
                    sourceAddress = instruction.UShortArg;
                    registers.D = (byte)memory[sourceAddress];
                    break;

                case OpCode.LoadX:
                    sourceAddress = instruction.UShortArg;
                    registers.X = (ushort)memory[sourceAddress];
                    break;

                case OpCode.DupA:
                    ushort targetAddress = instruction.UShortArg;
                    memory[targetAddress] = registers.A;
                    break;

                case OpCode.DupB:
                    targetAddress = instruction.UShortArg;
                    memory[targetAddress] = registers.B;
                    break;

                case OpCode.XchangeAB:
                    byte tmpByte = registers.B;
                    registers.B = registers.A;
                    registers.A = tmpByte;
                    break;

                case OpCode.XchangeCD:
                    tmpByte = registers.D;
                    registers.D = registers.C;
                    registers.D = tmpByte;
                    break;

                case OpCode.PushReg:
                    regId = (RegisterId)instruction.ByteArg1; //Should be ShortArg, but not sure about endianness
                    ushort regVal = 0x00;

                    switch (regId)
                    {
                        case RegisterId.A:
                            regVal = registers.A;
                            break;

                        case RegisterId.B:
                            regVal = registers.B;
                            break;

                        case RegisterId.C:
                            regVal = registers.C;
                            break;

                        case RegisterId.D:
                            regVal = registers.D;
                            break;

                        case RegisterId.X:
                            regVal = registers.X;
                            break;

                        case RegisterId.Y:
                            regVal = registers.Y;
                            break;

                        case RegisterId.AB:
                            regVal = registers.AB;
                            break;

                        case RegisterId.CD:
                            regVal = registers.CD;
                            break;
                    }
                    stackPointer++; //Pushing a new value on
                    memory[stackPointer] = regVal; //Add the value to the new point on the stack
                    break;

                case OpCode.PushA:
                    stackPointer++;
                    memory[stackPointer] = registers.A;
                    break;

                case OpCode.PushB:
                    stackPointer++;
                    memory[stackPointer] = registers.B;
                    break;

                case OpCode.PushC:
                    stackPointer++;
                    memory[stackPointer] = registers.C;
                    break;

                case OpCode.PushD:
                    stackPointer++;
                    memory[stackPointer] = registers.D;
                    break;

                case OpCode.PopReg:
                    regId = (RegisterId)instruction.ByteArg1;
                    ushort shortVal = (ushort)memory[stackPointer];
                    stackPointer--; //Shrink the stack

                    switch (regId)
                    {
                        case RegisterId.A:
                            registers.A = (byte)shortVal;
                            break;

                        case RegisterId.B:
                            registers.B = (byte)shortVal;
                            break;

                        case RegisterId.C:
                            registers.C = (byte)shortVal;
                            break;

                        case RegisterId.D:
                            registers.D = (byte)shortVal;
                            break;

                        case RegisterId.X:
                            registers.X = shortVal;
                            break;

                        case RegisterId.Y:
                            registers.Y = shortVal;
                            break;

                        case RegisterId.AB:
                            registers.AB = shortVal;
                            break;

                        case RegisterId.CD:
                            registers.CD = shortVal;
                            break;
                    }
                    break;

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
using HappyPenguinVM.Types;
using System;

namespace HappyPenguinVM.Execution
{
    public class ProgramExecutor
    {
        public int[] Memory => memory;

        private const int DEFAULT_MEMORY_SIZE = ushort.MaxValue; //65K cells, or 64KiB memory. The machine can actually address much more, as it has 32-bit registers.

        private int[] memory; //one memory cell is sizeof(int) big
        private int _memorySize;
        private Registers registers;

        private int programCounter; //Current instruction to execute
        private int stackPointer; //Position of stack

        //private int heapPointer; //Not used yet
        private int framePointer; //Points to start of stack frame

        private HappyPenguinVMProgram code;

        private bool skipProgramCounterUpdate; //Set to true to skip updating the program counter after executing the next instruction. This is set to false automatically after being checked.

        public ProgramExecutor(HappyPenguinVMProgram code) : this(code, 0)
        {
        }

        public ProgramExecutor(HappyPenguinVMProgram code, int memorySize)
        {
            if (memorySize < 0)
                throw new ArgumentOutOfRangeException(nameof(memorySize), "Negative memory size given");

            if (memorySize > uint.MaxValue)
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
                if (skipProgramCounterUpdate)
                {
                    skipProgramCounterUpdate = false;
                }
                else
                {
                    programCounter++;
                }
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
                    SetRegisterValueFromId(regId, val);
                    break;

                case OpCode.LoadA:
                    ushort sourceAddress = instruction.UShortArg1;
                    registers.A = (byte)memory[sourceAddress];
                    break;

                case OpCode.LoadB:
                    sourceAddress = instruction.UShortArg1;
                    registers.B = (byte)memory[sourceAddress];
                    break;

                case OpCode.LoadC:
                    sourceAddress = instruction.UShortArg1;
                    registers.C = (byte)memory[sourceAddress];
                    break;

                case OpCode.LoadD:
                    sourceAddress = instruction.UShortArg1;
                    registers.D = (byte)memory[sourceAddress];
                    break;

                case OpCode.LoadX:
                    sourceAddress = instruction.UShortArg1;
                    registers.X = (ushort)memory[sourceAddress];
                    break;

                case OpCode.DupA:
                    ushort targetAddress = instruction.UShortArg1;
                    memory[targetAddress] = registers.A;
                    break;

                case OpCode.DupB:
                    targetAddress = instruction.UShortArg1;
                    memory[targetAddress] = registers.B;
                    break;

                case OpCode.XchangeReg:
                    break;

                case OpCode.XchangeAB:
                    var tmpRg = registers.B;
                    registers.B = registers.A;
                    registers.A = tmpRg;
                    break;

                case OpCode.XchangeCD:
                    tmpRg = registers.D;
                    registers.D = registers.C;
                    registers.C = tmpRg;
                    break;

                case OpCode.PushReg:
                    regId = (RegisterId)instruction.ByteArg1; //Should be ShortArg, but not sure about endianness
                    uint regVal = GetRegisterValueFromId(regId);
                    stackPointer++; //Pushing a new value on
                    memory[stackPointer] = (int)regVal; //Add the value to the new point on the stack
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
                    uint shortVal = (uint)memory[stackPointer];
                    stackPointer--; //Shrink the stack
                    SetRegisterValueFromId(regId, shortVal);
                    break;

                case OpCode.PopA:
                    registers.A = (byte)memory[stackPointer];
                    stackPointer--;
                    break;

                case OpCode.PopB:
                    registers.B = (byte)memory[stackPointer];
                    stackPointer--;
                    break;

                case OpCode.PopC:
                    registers.C = (byte)memory[stackPointer];
                    stackPointer--;
                    break;

                case OpCode.PopD:
                    registers.D = (byte)memory[stackPointer];
                    stackPointer--;
                    break;

                case OpCode.Jump:
                    programCounter = instruction.UShortArg1;
                    skipProgramCounterUpdate = true;
                    break;

                case OpCode.JumpZ:
                    if (registers.ZF > 0)
                    {
                        programCounter = instruction.UShortArg1;
                        skipProgramCounterUpdate = true;
                    }
                    break;

                case OpCode.JumpNotZ:
                    if (registers.ZF == 0)
                    {
                        programCounter = instruction.UShortArg1;
                        skipProgramCounterUpdate = true;
                    }
                    break;

                case OpCode.CompareReg:
                    var regId1 = (RegisterId)instruction.ByteArg1;
                    var regId2 = (RegisterId)instruction.ByteArg2;
                    uint regVal1 = GetRegisterValueFromId(regId1);
                    uint regVal2 = GetRegisterValueFromId(regId2);

                    if (regVal1 == regVal2)
                    {
                        //Registers are equal
                        registers.ZF = 0x1;
                    }
                    break;

                case OpCode.Add:
                    regId1 = (RegisterId)instruction.ByteArg1;
                    regId2 = (RegisterId)instruction.ByteArg2;
                    regVal1 = GetRegisterValueFromId(regId1);
                    regVal2 = GetRegisterValueFromId(regId2);
                    var outputVal = regVal1 + regVal2;
                    SetRegisterValueFromId(regId1, outputVal);
                    break;

                case OpCode.Subtract:
                    regId1 = (RegisterId)instruction.ByteArg1;
                    regId2 = (RegisterId)instruction.ByteArg2;
                    regVal1 = GetRegisterValueFromId(regId1);
                    regVal2 = GetRegisterValueFromId(regId2);
                    outputVal = regVal1 - regVal2;
                    SetRegisterValueFromId(regId1, outputVal);
                    break;

                case OpCode.ShiftRight:
                    regId1 = (RegisterId)instruction.ByteArg1;
                    regId2 = (RegisterId)instruction.ByteArg2;
                    regVal1 = GetRegisterValueFromId(regId1);
                    regVal2 = GetRegisterValueFromId(regId2);
                    outputVal = (uint)((int)regVal1 >> (int)regVal2);
                    SetRegisterValueFromId(regId1, outputVal);
                    break;

                case OpCode.ShiftLeft:
                    regId1 = (RegisterId)instruction.ByteArg1;
                    regId2 = (RegisterId)instruction.ByteArg2;
                    regVal1 = GetRegisterValueFromId(regId1);
                    regVal2 = GetRegisterValueFromId(regId2);
                    outputVal = (uint)((int)regVal1 << (int)regVal2);
                    SetRegisterValueFromId(regId1, outputVal);
                    break;

                case OpCode.Call:
                    framePointer = stackPointer; //Start of current frame
                    var tmp = programCounter;
                    programCounter = memory[stackPointer];
                    skipProgramCounterUpdate = true;
                    memory[stackPointer] = tmp;
                    break;

                case OpCode.Return:
                    programCounter = memory[framePointer]; //Get old program counter from previous frame
                    skipProgramCounterUpdate = false; //We actually want to go to the line after the call instruction
                    stackPointer = framePointer - instruction.ByteArg1;
                    framePointer = memory[framePointer - 1]; //TODO: Necessary? Review this
                    break;

                case OpCode.Nop:
                    //do nothing...
                    break;

                case OpCode.Fault:
                    var faultCode = instruction.UShortArg1;
                    break;

                case OpCode.Halt:
                    //do nothing, will be handled by main excecution cycle
                    break;

                default:
                    throw new InvalidOpCodeException("An unknown opcode was encountered: ", instruction.OpCode);
            }
        }

        #region Private Utility Functions

        /// <summary>
        /// Returns a register's value from its id
        /// </summary>
        /// <param name="regId"></param>
        /// <returns></returns>
        private uint GetRegisterValueFromId(RegisterId regId)
        {
            uint regVal = 0x00;

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

                case RegisterId.R:
                    regVal = registers.R;
                    break;

                case RegisterId.S:
                    regVal = registers.S;
                    break;

                case RegisterId.AB:
                    regVal = registers.AB;
                    break;

                case RegisterId.CD:
                    regVal = registers.CD;
                    break;
            }
            return regVal;
        }

        /// <summary>
        /// Set a register's value
        /// </summary>
        /// <param name="regId"></param>
        /// <param name="value"></param>
        private void SetRegisterValueFromId(RegisterId regId, uint value)
        {
            switch (regId)
            {
                case RegisterId.A:
                    registers.A = (byte)value;
                    break;

                case RegisterId.B:
                    registers.B = (byte)value;
                    break;

                case RegisterId.C:
                    registers.C = (byte)value;
                    break;

                case RegisterId.D:
                    registers.D = (byte)value;
                    break;

                case RegisterId.E:
                    registers.E = (byte)value;
                    break;

                case RegisterId.F:
                    registers.F = (byte)value;
                    break;

                case RegisterId.G:
                    registers.G = (byte)value;
                    break;

                case RegisterId.H:
                    registers.H = (byte)value;
                    break;

                case RegisterId.R:
                    registers.R = (ushort)value;
                    break;

                case RegisterId.S:
                    registers.S = (ushort)value;
                    break;

                case RegisterId.T:
                    registers.T = (ushort)value;
                    break;

                case RegisterId.U:
                    registers.U = (ushort)value;
                    break;

                case RegisterId.W:
                    registers.W = value;
                    break;

                case RegisterId.X:
                    registers.X = value;
                    break;

                case RegisterId.Y:
                    registers.Y = value;
                    break;

                case RegisterId.Z:
                    registers.Z = value;
                    break;

                case RegisterId.AB:
                    registers.AB = (ushort)value;
                    break;

                case RegisterId.CD:
                    registers.CD = (ushort)value;
                    break;

                case RegisterId.EF:
                    registers.EF = (ushort)value;
                    break;

                case RegisterId.GH:
                    registers.GH = (ushort)value;
                    break;

                case RegisterId.ABCD:
                    registers.ABCD = value;
                    break;

                case RegisterId.EFGH:
                    registers.ABCD = value;
                    break;

                case RegisterId.RS:
                    registers.ABCD = value;
                    break;

                case RegisterId.TU:
                    registers.ABCD = value;
                    break;
            }
        }

        #endregion Private Utility Functions
    }
}
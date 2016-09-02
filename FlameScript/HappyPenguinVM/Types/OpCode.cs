namespace HappyPenguinVM.Types
{
    /// <summary>
    /// OpCode in the HappyPenguinVM machine.
    /// Uses one byte.
    /// </summary>
    /// <remarks>
    /// Changing a value here will break existing
    /// code files. That why there is a explicit value
    /// for every OpCode, so that we do not rely on
    /// the order of the elements.
    /// DO NOT change the value of one of this members
    /// until absoulutly necessary!
    /// </remarks>
    public enum OpCode : byte
    {
        MovReg = 0x10, //Move a value into a register (ex: movreg a,2). RegisterId's are in RegisterId struct
        LoadA = 0x11,
        LoadB = 0x12,
        LoadC = 0x13,
        LoadD = 0x14,
        LoadX = 0x15,
        DupA = 0x16, //Reverse of LoadA (copy A into that memory location) [ex: dupa 0xffff]. This copies the value of A into 0xffff
        DupB = 0x17,
        XchangeReg = 0x18, //Exchange two registers
        XchangeAB = 0x19, //Exchange A and B
        XchangeCD = 0x1A, //Exchange C andD
        

        PushReg = 0x20,
        PushA = 0x21,
        PushB = 0x22,
        PushC = 0x23,
        PushD = 0x24,
        PopReg = 0x25,
        PopA = 0x26,
        PopB = 0x27,
        PopC = 0x28,
        PopD = 0x29,

        Jump = 0x30, //Jump to a location specified by the short arg
        JumpZ = 0x31, //Jump if the ZF is set
        JumpNotZ = 0x32, //Jump if the ZF is not set
        JumpFromStack = 0x33,

        CompareReg = 0x40, //Compares two registers (specified with IDs through byteargs) and sets ZF if they are equal

        //5: Arithmetic and shifting!

        Add = 0x50, //Add registers from Id's byteArg1 and byteArg2. Result put in first register.
        Subtract = 0x51, //Subtract registers from Id's byteArg1 and byteArg2
        ShiftRight = 0x52,
        ShiftLeft = 0x53,

        Call = 0x80, //Call a section of code. Location will be read from top of the stack
        Return = 0x8A,

        //9: Alloc-like stuff?

        Nop = 0x00,
        Halt = 0xFF,
    }
}
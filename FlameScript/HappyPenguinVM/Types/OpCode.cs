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
        LoadC = 0x10,
        Load = 0x11,
        LoadA = 0x12,
        Dup = 0x13,
        Mov = 0x14,

        Pop = 0x20,
        Push = 0x21,
        PushA = 0x22,

        Jump = 0x30,
        JumpZ = 0x31,
        JumpI = 0x32,

        Add = 0x40,
        Sub = 0x41,
        Mul = 0x42,
        Div = 0x43,
        Mod = 0x44,

        Neg = 0x50,

        Eq = 0x60,
        Neq = 0x61,
        Le = 0x62,
        Leq = 0x63,
        Gr = 0x64,
        Geq = 0x65,

        And = 0x70,
        Or = 0x71,
        Not = 0x72,

        Mark = 0x80,
        Call = 0x81,
        Enter = 0x82,
        Alloc = 0x83,
        Slide = 0x84,
        Return = 0x85,

        New = 0x90,

        Nop = 0x00,
        Halt = 0xFF,
    }
}
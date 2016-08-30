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
        Move = 0x10,
        LoadA = 0x11,
        LoadB = 0x12,
        LoadC = 0x13,
        LoadD = 0x14,
        LoadX = 0x15,

        Pop = 0x20,
        Push = 0x21,
        PushA = 0x22,
        PushB = 0x23,
        PushC = 0x24,
        PushD = 0x25,
        PopA = 0x26,
        PopB = 0x27,
        PopC = 0x28,
        PopD = 0x29,

        Jump = 0x30,
        JumpZ = 0x31,
        JumpNotZ = 0x32,

        Compare = 0x40,

        Call = 0x80,
        Return = 0x81,

        //9: Alloc-like stuff?

        Nop = 0x00,
        Halt = 0xFF,
    }
}
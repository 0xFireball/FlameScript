﻿namespace HappyPenguinVM.Types
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
        XchangeAB = 0x18, //Exchange A and B
        XchangeCD = 0x19, //Exchange C andD

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
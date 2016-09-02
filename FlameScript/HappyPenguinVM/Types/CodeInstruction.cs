using System.Runtime.InteropServices;

namespace HappyPenguinVM.Types
{
    /// <summary>
    /// Single instruction in HappyPenguinVM machine code.
    /// Consists of one eight bit opcode and either a 16 bit
    /// argument or two eight bit arguments.
    /// This is a struct, using LayoutKind.Explicit!
    /// </summary>
    /// <remarks>
    /// Convention:
    /// When two args are needed, both have to be bytes.
    /// If the only arg is a memory address it is a short
    /// (to allow addressing the complete memory), when it
    /// is a counter/amount, it is a byte.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    public struct CodeInstruction
    {
        [FieldOffset(0)]
        public OpCode OpCode;

        [FieldOffset(1)]
        public byte ByteArg1;

        [FieldOffset(2)]
        public byte ByteArg2;

        [FieldOffset(3)]
        public byte ByteArg3;

        [FieldOffset(4)]
        public byte ByteArg4;

        [FieldOffset(1)] //overlay over ByteArg1/2
        public ushort UShortArg1;

        [FieldOffset(3)] //overlay over ByteArg3/4
        public ushort UShortArg2;

        [FieldOffset(1)]
        public uint UIntArg1;

        [FieldOffset(5)]
        public uint UIntArg2;
    }
}
using System.Runtime.InteropServices;

namespace HappyPenguinVM.Types
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Registers
    {
        [FieldOffset(0)]
        public byte A;

        [FieldOffset(1)]
        public byte B;

        [FieldOffset(2)]
        public byte C;

        [FieldOffset(3)]
        public byte D;

        [FieldOffset(0)]
        public ushort AB;

        [FieldOffset(2)]
        public ushort CD;

        [FieldOffset(4)]
        public ushort X;

        [FieldOffset(6)]
        public ushort Y;
    }
}
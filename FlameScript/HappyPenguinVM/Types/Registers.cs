using System.Runtime.InteropServices;

namespace HappyPenguinVM.Types
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Registers
    {
        //8-bit registers
        [FieldOffset(0)]
        public byte A;

        [FieldOffset(1)]
        public byte B;

        [FieldOffset(2)]
        public byte C;

        [FieldOffset(3)]
        public byte D;

        [FieldOffset(4)]
        public byte E;

        [FieldOffset(5)]
        public byte F;

        [FieldOffset(6)]
        public byte G;

        [FieldOffset(7)]
        public byte H;

        //16-bit single registers

        [FieldOffset(8)]
        public ushort R;

        [FieldOffset(10)]
        public ushort S;

        [FieldOffset(12)]
        public ushort T;

        [FieldOffset(14)]
        public ushort U;

        //32-bit single registers
        [FieldOffset(16)]
        public uint W;

        [FieldOffset(20)]
        public uint X;

        [FieldOffset(24)]
        public uint Y;

        [FieldOffset(28)]
        public uint Z;

        //16-bit combined registers

        [FieldOffset(0)]
        public ushort AB;

        [FieldOffset(2)]
        public ushort CD;

        [FieldOffset(4)]
        public ushort EF;

        [FieldOffset(6)]
        public ushort GH;

        //32-bit combined registers

        [FieldOffset(0)]
        public uint ABCD;

        [FieldOffset(4)]
        public uint EFGH;

        [FieldOffset(8)]
        public uint RS;

        [FieldOffset(12)]
        public uint TU;

        //Flags

        [FieldOffset(32)]
        public byte ZF; //Zero Flag

        [FieldOffset(33)]
        public byte OF; //Overflow Flag
    }
}
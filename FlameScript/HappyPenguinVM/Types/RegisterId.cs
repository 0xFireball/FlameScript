namespace HappyPenguinVM.Types
{
    public enum RegisterId : byte
    {
        // Byte registers (8-bit)
        A = 0x01,

        B = 0x02,

        C = 0x03,

        D = 0x04,

        E = 0x05,

        F = 0x06,

        G = 0x07,

        H = 0x08,

        // Short registers (16-bit)
        R = 0x10,

        S = 0x11,

        T = 0x12,

        U = 0x13,

        // Combined registers (16-bit)
        AB = 0x20,

        CD = 0x21,

        EF = 0x22,

        GH = 0x23,

        // UInt registers (32-bit)
        W = 0x30,

        X = 0x31,

        Y = 0x32,

        Z = 0x33,

        // Combined registers (32-bit)
        ABCD = 0x40,

        EFGH = 0x41,

        RS = 0x42,

        TU = 0x43,

        // Flags (byte)

        ZF = 0xA0,

        OF = 0xA1,
    }
}
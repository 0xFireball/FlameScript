namespace HappyPenguinVM.Types
{
    public enum RegisterId : byte
    {
        // Byte registers (8-bit)
        A = 0x01,

        B = 0x02,
        C = 0x03,
        D = 0x04,

        // Short registers (16-bit)
        X = 0x10,

        Y = 0x11,

        // Combined registers (16-bit)
        AB = 0x20,

        CD = 0x21,

        // Flags (byte)

        ZF = 0x40,
    }
}
namespace HappyPenguinVM.Types
{
    public enum RegisterId : byte
    {
        A = 0x01,
        B = 0x02,
        C = 0x03,
        D = 0x04,

        X = 0x10,
        Y = 0x11,

        AB = 0x20,
        CD = 0x21,

        Z = 0x40,
    }
}

# HappyPenguinVM

HappyPenguinVM is an **insanely experimental** bytecode VM. It runs its own assembly-like list of instructions, but has
quite a few special quirks.

## Some Quirks

### Registers
- 8 8-bit registers (A,B,C,D,E,F,G,H)
- 4 16-bit registers (R,S,T,U)
- 4 32-bit registers (W,X,Y,Z)
- 4 16-bit combined registers (AB, CD, EF, GH)
- 4 32-bit combined registers (ABCD, EFGH, RS, TU)

Basically, the machine supports 8-bit, 16-bit, and 32-bit, somewhat simultaneously. Look at the instruction format below to understand
why.

### Instructions
- 9 bytes in size
- 1 byte is the OpCode
- 8 of the bytes are made up by 2 `uint` arguments, 4 bytes each. But wait. That's not all!
  - The first `uint` argument is made up of two `ushort` arguments. You can just specify two `ushorts` when creating an instruction and the second `uint` argument will be empty.
  - The first `ushort` (yes, the `ushort`) can be specified with 2 `byte` arguments instead.
  - The arguments overlap, so you can specify everything simultaneously with the optional 2 `uint` form.
- Essentially, you can give instructions parameters in one of the following forms:
  - 2 `byte` parameters
  - 2 `ushort` parameters (you can also assign the `byte` parameters)
  - 2 `uint` parameters (you can also assign `ushort` and `byte` parameters)

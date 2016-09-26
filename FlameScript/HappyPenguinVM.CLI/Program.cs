using HappyPenguinVM.Encoding;
using HappyPenguinVM.Execution;
using HappyPenguinVM.Types;
using System.IO;

namespace HappyPenguinVM.CLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            /*
            if (args.Length == 0)
            {
                Console.WriteLine("No input file specified");
                return;
            }

            var sourcePath = args[0];
            */
            var codeEmitter = new HappyPenguinCodeEmitter();

            /*
            codeEmitter.Emit(OpCode.PushA);
            codeEmitter.Emit(OpCode.Load, 0xFF, 0xFF);
            codeEmitter.Emit(OpCode.Load, 0xFFFF);
            codeEmitter.Emit(OpCode.Load, 0xAF, 0xAF);
            codeEmitter.Emit(OpCode.Load, 0xAFAF);
            codeEmitter.Emit(OpCode.Return);
            */

            //The # represents the programCounter number of the below program
            codeEmitter.Emit(OpCode.Nop); //0.
            codeEmitter.Emit(OpCode.MovReg, (byte)RegisterId.A, 0x44); //1. Set A to 0x44
            codeEmitter.Emit(OpCode.DupA, 0xFFAA); //2. Copy the data in A to 0xFFAA in memory
            codeEmitter.Emit(OpCode.LoadB, 0xFFAA); //3. Copy the data at 0xFFAA from A to B
            codeEmitter.Emit(OpCode.PushA); //4. Push A's value onto the stack
            codeEmitter.Emit(OpCode.MovReg, (byte)RegisterId.A, 0x33); //5. Set A to 0x33
            codeEmitter.Emit(OpCode.XchangeAB); //6. Exchange the values of A and B
            codeEmitter.Emit(OpCode.PopA); //7. Pop the stored value of A from the stack back into A
            codeEmitter.Emit(OpCode.MovReg, (byte)RegisterId.A, 0x22); //8. Set A to 0x22
            codeEmitter.Emit(OpCode.CompareReg, (byte)RegisterId.A, (byte)RegisterId.B); //9. Compare A and B
            codeEmitter.Emit(OpCode.JumpZ, 0xE); //10. Jump to PC=0x if the registers in the previous comparison were equal
            codeEmitter.Emit(OpCode.DupA, 0xFFAA); //11. Copy the data in A to 0xFFAA in memory
            codeEmitter.Emit(OpCode.LoadB, 0xFFAA); //12. Copy the data at 0xFFAA from A to B
            codeEmitter.Emit(OpCode.Jump, 0x9); //13. Jump back to line 0x9 to do comparison again
            codeEmitter.Emit(OpCode.Nop); //14. Jump target from 10.
            codeEmitter.Emit(OpCode.Jump, 0x14); //15. Jump to after function
            codeEmitter.Emit(OpCode.PopA); //16. Retrieve A parameter
            codeEmitter.Emit(OpCode.Nop); //17. Temporary placeholder
            codeEmitter.Emit(OpCode.Nop); //18. Temporary placeholder
            codeEmitter.Emit(OpCode.Return); //19. Return from function
            codeEmitter.Emit(OpCode.Nop); //20. Jump target from 14.
            codeEmitter.Emit(OpCode.MovReg, (byte)RegisterId.A, 0x4); //21. Set A to 0x4
            codeEmitter.Emit(OpCode.ShiftLeft, (byte)RegisterId.A, 0x2); //22. Do a simple multiply by 4 with shifting
            codeEmitter.Emit(OpCode.PushA); //23. Store A
            codeEmitter.Emit(OpCode.PopB); //24. Restore B from stack
            codeEmitter.Emit(OpCode.ShiftRight, (byte)RegisterId.A, 0x2); //25. Undo the multiply
            codeEmitter.Emit(OpCode.CompareReg, (byte)RegisterId.A, (byte)RegisterId.B); //26. Compare A and B
            codeEmitter.Emit(OpCode.JumpZ, 0x1D); //27. Jump to PC=1d if the registers in the previous comparison were equal
            codeEmitter.Emit(OpCode.Fault, 0x1); //28. Fault with code 0x1
            codeEmitter.Emit(OpCode.Nop); //29. Jump target
            codeEmitter.Emit(OpCode.MovReg, (byte)RegisterId.A, 0x11); //30. Load 0x11 into A as the call target
            codeEmitter.Emit(OpCode.PushA); //31. Push the A register onto the stack as the call target
            codeEmitter.Emit(OpCode.Call); //32. Call the function from the address on the stack
            codeEmitter.Emit(OpCode.Halt); //End of program. Halt.

            var vmProgram = codeEmitter.GetEmittedCode();
            var codeEncoder = new CodeEncoder();
            using (var outputFileStream = File.Open("testprogram.🐧", FileMode.Create))
            {
                var outStream = new MemoryStream();
                codeEncoder.EncodeCodeToStream(vmProgram, outStream);
                outStream.Position = 0;
                var loadedProgram = codeEncoder.ReadCodeFromStream(outStream);
                outStream.Position = 0;
                outStream.CopyTo(outputFileStream);
            }

            var executor = new ProgramExecutor(vmProgram);
            executor.InitializeMachine();
            executor.ExecuteCode();
            //Dump memory
            /*
            for (int i = 0; i < executor.Memory.Length; i++)
            {
                Console.Write($"{executor.Memory[i]} ");
            }
            */
        }
    }
}
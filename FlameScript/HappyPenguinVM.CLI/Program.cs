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

            codeEmitter.Emit(OpCode.MovReg, (byte)RegisterId.A, 0x44);
            codeEmitter.Emit(OpCode.DupA, 0xFFAA);
            codeEmitter.Emit(OpCode.LoadB, 0xFFAA);
            codeEmitter.Emit(OpCode.PushA);
            codeEmitter.Emit(OpCode.MovReg, (byte)RegisterId.A, 0x33);
            codeEmitter.Emit(OpCode.XchangeAB);
            codeEmitter.Emit(OpCode.PopA);
            codeEmitter.Emit(OpCode.Halt);

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
using HappyPenguinVM.Encoding;
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
            codeEmitter.Emit(OpCode.PushA);
            codeEmitter.Emit(OpCode.Load, 0xFF, 0xFF);
            codeEmitter.Emit(OpCode.Load, 0xFFFF);
            codeEmitter.Emit(OpCode.Load, 0xAF, 0xAF);
            codeEmitter.Emit(OpCode.Load, 0xAFAF);
            codeEmitter.Emit(OpCode.Return);
            var vmProgram = codeEmitter.GetEmittedCode();
            var codeEncoder = new CodeEncoder();
            var ofStream = File.Open("testprogram.🐧", FileMode.Create);
            codeEncoder.EncodeCodeToStream(vmProgram, ofStream);
        }
    }
}
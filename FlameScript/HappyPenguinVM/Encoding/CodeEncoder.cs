using HappyPenguinVM.Types;
using System.IO;

namespace HappyPenguinVM.Encoding
{
    /// <summary>
    /// A class that helps load and save HappyPenguinVM machine code programs
    /// </summary>
    public class CodeEncoder
    {
        public static string MagicHeader { get; } = "☺🐧☺";

        public void EncodeCodeToStream(HappyPenguinVMProgram programCode, Stream outputStream)
        {
            var outputStreamWriter = new StreamWriter(outputStream);
            //Write magic header
            outputStreamWriter.Write(MagicHeader);
            foreach (var instruction in programCode)
            {
                outputStreamWriter.Write(instruction.OpCode);
                outputStreamWriter.Write(instruction.ByteArg1);
                outputStreamWriter.Write(instruction.ByteArg2);
            }
        }

        public HappyPenguinVMProgram ReadCodeFromStream(Stream inputStream)
        {
            var inputStreamReader = new StreamReader(inputStream);
            var inputData = inputStreamReader.ReadToEnd();
            return new HappyPenguinVMProgram();
        }
    }
}
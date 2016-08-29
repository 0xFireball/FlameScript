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
            outputStreamWriter.Flush();
        }

        public HappyPenguinVMProgram ReadCodeFromStream(Stream inputStream)
        {
            var inputStreamReader = new StreamReader(inputStream);
            //var inputData = inputStreamReader.ReadToEnd();

            //Verify header
            char[] headerBuffer = new char[MagicHeader.Length];
            inputStreamReader.ReadBlock(headerBuffer, 0, headerBuffer.Length);
            var readHeaderString = new string(headerBuffer);
            var headerMatch = readHeaderString == MagicHeader;
            if (!headerMatch)
            {
                throw new InvalidDataException("The data does not contain a valid HappyPenguinVM program!");
            }
            //Header matches, now read instructions into code
            var readProgram = new HappyPenguinVMProgram();
            while (!inputStreamReader.EndOfStream)
            {
                inputStreamReader.Read();
            }

            return new HappyPenguinVMProgram();
        }
    }
}
using HappyPenguinVM.Types;
using System;
using System.IO;
using System.Linq;

namespace HappyPenguinVM.Encoding
{
    /// <summary>
    /// A class that helps load and save HappyPenguinVM machine code programs
    /// </summary>
    public class CodeEncoder
    {
        public static byte[] MagicHeader { get; } = new byte[] { 0xF0, 0x9F, 0x90, 0xA7 }; //"🐧"

        public void EncodeCodeToStream(HappyPenguinVMProgram programCode, Stream outputStream)
        {
            var outputStreamWriter = new BinaryWriter(outputStream);
            //Write magic header
            outputStreamWriter.Write(MagicHeader);
            foreach (var instruction in programCode)
            {
                outputStreamWriter.Write((byte)instruction.OpCode);
                outputStreamWriter.Write(instruction.UIntArg1);
                outputStreamWriter.Write(instruction.UIntArg2);
            }
            outputStreamWriter.Flush();
        }

        public HappyPenguinVMProgram ReadCodeFromStream(Stream inputStream)
        {
            var inputStreamReader = new BinaryReader(inputStream);
            //var inputData = inputStreamReader.ReadToEnd();

            //Verify header
            byte[] headerBuffer = new byte[MagicHeader.Length];
            headerBuffer = inputStreamReader.ReadBytes(headerBuffer.Length);
            var headerMatch = MagicHeader.SequenceEqual(headerBuffer);
            if (!headerMatch)
            {
                throw new InvalidProgramException("The data does not contain a valid HappyPenguinVM program!");
            }
            //Header matches, now read instructions into code
            var readProgram = new HappyPenguinVMProgram();
            byte[] instructionReaderBuffer = new byte[sizeof(byte) + sizeof(int) * 2]; //Size of the instruction (byte + int + int)
            while (inputStreamReader.BaseStream.Position < inputStreamReader.BaseStream.Length)
            {
                inputStreamReader.Read(instructionReaderBuffer, 0, instructionReaderBuffer.Length);
                byte opCodeByte = instructionReaderBuffer[0];
                uint uiArg1 = (uint)BitConverter.ToInt32(instructionReaderBuffer, 1);
                uint uiArg2 = (uint)BitConverter.ToInt32(instructionReaderBuffer, 5);
                readProgram.Add(new CodeInstruction { OpCode = (OpCode)uiArg1, UIntArg1 = uiArg1, UIntArg2 = uiArg2 });
            }

            return readProgram;
        }
    }
}
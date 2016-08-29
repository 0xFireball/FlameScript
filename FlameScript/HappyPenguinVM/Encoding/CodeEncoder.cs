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
                outputStreamWriter.Write((int)instruction.OpCode);
                outputStreamWriter.Write(instruction.ByteArg1);
                outputStreamWriter.Write(instruction.ByteArg2);
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
                throw new InvalidDataException("The data does not contain a valid HappyPenguinVM program!");
            }
            //Header matches, now read instructions into code
            var readProgram = new HappyPenguinVMProgram();
            byte[] instructionReaderBuffer = new byte[sizeof(int) + sizeof(short)]; //Size of the instruction (int + short)
            while (inputStreamReader.BaseStream.Position < inputStreamReader.BaseStream.Length)
            {
                inputStreamReader.Read(instructionReaderBuffer, 0, instructionReaderBuffer.Length);
                int opCodeInt = BitConverter.ToInt32(instructionReaderBuffer, 0);
                byte arg1 = instructionReaderBuffer[4]; //Get the fifth byte
                byte arg2 = instructionReaderBuffer[5]; //Get the sixth byte
                readProgram.Add(new CodeInstruction { OpCode = (OpCode)opCodeInt, ByteArg1 = arg1, ByteArg2 = arg2 });
            }

            return readProgram;
        }
    }
}
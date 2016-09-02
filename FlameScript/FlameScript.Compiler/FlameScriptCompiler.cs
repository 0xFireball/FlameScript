using FlameScript.Compiler.AstEmitters;
using FlameScript.Types.Ast;
using HappyPenguinVM;
using HappyPenguinVM.Encoding;
using HappyPenguinVM.Types;
using System.IO;

namespace FlameScript.Compiler
{
    public class FlameScriptCompiler
    {
        private ProgramNode _programAst;

        public FlameScriptCompiler(ProgramNode ast)
        {
            _programAst = ast;
        }

        public byte[] CompileProgram()
        {
            var astCompiler = new AstCompiler(_programAst);
            var emitter = astCompiler.CreateEmitter();
            var vmCodeEmitter = new HappyPenguinCodeEmitter();

            emitter.EmitCode(vmCodeEmitter); //This will emit code to the code emitter object
            vmCodeEmitter.Emit(OpCode.Halt); //Add a final halt
            var compiledProgram = vmCodeEmitter.GetEmittedCode();
            var codeEncoder = new CodeEncoder();

            

            byte[] compiledProgramBytes;
            using (var outStream = new MemoryStream())
            {
                codeEncoder.EncodeCodeToStream(compiledProgram, outStream);
                compiledProgramBytes = compiledProgramBytes = outStream.ToArray();
            }
            return compiledProgramBytes;
        }
    }
}
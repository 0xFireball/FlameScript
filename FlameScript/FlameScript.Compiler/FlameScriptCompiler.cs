using FlameScript.Compiler.AstEmitters;
using FlameScript.Types.Ast;

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
            return new byte[0];
        }
    }
}
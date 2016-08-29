using FlameScript.Compiler;
using FlameScript.Lexing;
using FlameScript.Parsing;
using FlameScript.Runtime.Interpreter;
using System;
using System.IO;

namespace FlameScript.CLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No input file specified");
                return;
            }
            var scriptPath = args[0];
            var useCompiler = args.Length > 1 && args[1] == "--compiler";

            var code = File.ReadAllText(scriptPath);

            //Tokenize the code
            var tokenizer = new Tokenizer(code);
            var tokens = tokenizer.Tokenize();

            //Parse the tokenized code, and create an AST
            var parser = new Parser(tokens);
            var ast = parser.ParseToAst();

            if (useCompiler)
            {
                //Use interpreter

                //Create an interpreter and run the code
                var interpreter = new FlameScriptInterpreter(ast);
                interpreter.DefineFunction("print", new Action<string>(Console.WriteLine));
                interpreter.ExecuteProgram();
            }
            else
            {
                //Use compiler
                var compiler = new FlameScriptCompiler(ast);
                compiler.CompileProgram();
            }
        }
    }
}
using FlameScript.Lexing;
using FlameScript.Parsing;
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

            var code = File.ReadAllText(scriptPath);

            //Tokenize the code
            var tokenizer = new Tokenizer(code);
            var tokens = tokenizer.Tokenize();

            //Parse the tokenized code, and create an AST
            var parser = new Parser(tokens);
            var ast = parser.ParseToAst();
        }
    }
}
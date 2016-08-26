using FlameScript.Lexing;
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
            var tokenizer = new Tokenizer(code);
            var tokens = tokenizer.Tokenize();
        }
    }
}
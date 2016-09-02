using FlameScript.Compiler;
using FlameScript.Lexing;
using FlameScript.Parsing;
using Nancy;
using Newtonsoft.Json;
using System;

namespace FlameScript.Web.Modules
{
    public class CodeProcessorModule : NancyModule
    {
        public CodeProcessorModule()
        {
            Get("/", _ =>
            {
                return View["Editor"];
            });
            Post("/processCode", (parameters) =>
            {
                /*
                var postDataStream = Request.Body;
                var postDataReader = new StreamReader(postDataStream);
                var rawPostData = postDataReader.ReadToEnd();
                */

                var code = Request.Form["code"];
                var compileMode = Request.Form["compileMode"];

                //Tokenize the code
                var tokenizer = new Tokenizer(code);
                var tokens = tokenizer.Tokenize();

                //Parse the tokenized code, and create an AST
                var parser = new Parser(tokens);
                var ast = parser.ParseToAst();

                if (compileMode == "ast")
                {
                    //Just give back the ast
                    var astJson = JsonConvert.SerializeObject(ast);
                    return astJson;
                }
                else if (compileMode == "compiler")
                {
                    //Use compiler
                    var compiler = new FlameScriptCompiler(ast);
                    var compiledProgram = compiler.CompileProgram();
                    var compiledProgramb64 = Convert.ToBase64String(compiledProgram);
                    return compiledProgramb64;
                }
                return "Compile mode not specified.";
            });
        }
    }
}
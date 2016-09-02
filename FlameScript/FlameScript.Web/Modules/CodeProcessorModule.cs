using FlameScript.Lexing;
using FlameScript.Parsing;
using Nancy;
using Newtonsoft.Json;

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
                //Tokenize the code
                var tokenizer = new Tokenizer(code);
                var tokens = tokenizer.Tokenize();

                //Parse the tokenized code, and create an AST
                var parser = new Parser(tokens);
                var ast = parser.ParseToAst();
                var astJson = JsonConvert.SerializeObject(ast);
                return astJson;
            });
        }
    }
}
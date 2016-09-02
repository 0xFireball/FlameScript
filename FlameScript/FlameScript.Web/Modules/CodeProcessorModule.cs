using Nancy;
using System.IO;

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
                var postDataStream = Request.Body;
                var postDataReader = new StreamReader(postDataStream);
                var rawPostData = postDataReader.ReadToEnd();
                return "Hello, World! 2.0";
            });
        }
    }
}
using Nancy.Hosting.Self;
using System;

namespace FlameScript.Web
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var port = 4077;
            var nancyHostUri = new Uri($"http://localhost:{port}");
            var hostConf = new HostConfiguration { RewriteLocalhost = false };

            using (var host = new NancyHost(hostConf, nancyHostUri))
            {
                host.Start();
                Console.WriteLine($"Application server running on {nancyHostUri}");
                Console.ReadLine();
            }
        }
    }
}
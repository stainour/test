using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static IWebHost BuildWebHost(string[] args) => WebHost.CreateDefaultBuilder(args).ConfigureLogging((context, builder) => builder.AddConsole())
            .UseStartup<Startup>()
            .Build();

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }
    }
}
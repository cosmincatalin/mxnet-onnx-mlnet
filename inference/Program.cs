using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace inference
{
    internal static class Program
    {
        private static void Main(string[] args) => 
            WebHost
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration((_, config) => config.AddCommandLine(args))
                .UseStartup<Startup>()
                .UseKestrel()
                .Build()
                .Run();
    }
}

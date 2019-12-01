using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace MemeMachine
{
    class Program
    {
        public static IConfiguration ProjectConfiguration;
        public static string Token;
        public static void Main(string[] args)
        {
            Token = args[0];
            ProjectConfiguration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
        }
    }
}

using HospitalMQ.Models;
using HospitalMQ.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HospitalMQ
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("config.json");
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<AppDbContext>();
                    services.AddSingleton<StorageService>();
                    services.AddHostedService<MessageConsumeService>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}

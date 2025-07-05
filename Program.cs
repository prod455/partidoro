using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Toolkit.Uwp.Notifications;
using Partidoro.Application.Cli;
using Partidoro.Application.Windows;
using System.Reflection;

namespace Pomodoro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
            }

            builder.Configuration.AddEnvironmentVariables();

            Startup startup = new Startup();
            startup.ConfigureServices(builder.Configuration, builder.Services);

            IHost host = builder.Build();

            host.Services.GetRequiredService<CliApplication>().Run(args);
        }
    }
}

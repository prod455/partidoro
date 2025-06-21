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
            ToastNotificationManagerCompat.OnActivated += (e) =>
            {
                ToastArguments args = ToastArguments.Parse(e.Argument);

                if (args.TryGetValue("action", out string action) && action == "open")
                {
                    WindowsApi.FocusConsole();
                }
            };

            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
            }

            builder.Configuration.AddEnvironmentVariables();

            Startup startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            IHost host = builder.Build();

            host.Services.GetRequiredService<CliApplication>().Run(args);
        }
    }
}

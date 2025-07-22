using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Partidoro.Application.Cli;
using Partidoro.EntityFrameworkCore;
using Partidoro.Services;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;

namespace Pomodoro
{
    public class Startup
    {
        public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            string connectionString = configuration.GetConnectionString("PomodoroDbConnection") ?? throw new ApplicationException("Missing ConnectionStrings configuration");

            services.AddSingleton<CommandApp>(provider =>
            {
                ServiceCollection serviceCollection = new ServiceCollection();

                serviceCollection.AddDbContext<PartidoroDbContext>(options =>
                {
                    options.UseSqlServer(connectionString, options =>
                    {
                        options.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                        );
                    });

                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                });

                serviceCollection.AddTransient<ProjectService>();
                serviceCollection.AddTransient<TaskService>();
                serviceCollection.AddTransient<RecordService>();

                return new CommandApp(new DependencyInjectionRegistrar(serviceCollection));
            });

            services.AddSingleton<CliApplication>();
        }
    }
}

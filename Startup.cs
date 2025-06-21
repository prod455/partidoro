using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Partidoro.Application.Cli;
using Partidoro.Application.Cli.Commands;
using Partidoro.EntityFrameworkCore;
using Partidoro.Services;
using Spectre.Console.Cli;

namespace Pomodoro
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = _configuration.GetConnectionString("PomodoroDbConnection") ?? throw new ApplicationException("Missing ConnectionStrings configuration");

            services.AddSingleton<CommandApp>(provider =>
            {
                ServiceCollection serviceCollection = new ServiceCollection();

                serviceCollection.AddDbContext<PartidoroDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);

                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                });

                serviceCollection.AddTransient<ProjectService>();
                serviceCollection.AddTransient<TaskService>();
                serviceCollection.AddTransient<RecordService>();

                TypeRegistrar dependencyInjectionRegistrar = new TypeRegistrar(serviceCollection);

                CommandApp app = new CommandApp(dependencyInjectionRegistrar);
                return app;
            });

            services.AddSingleton<CliApplication>();
        }
    }
}

using Partidoro.Domain;
using Partidoro.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Partidoro.Application.Cli.Commands
{
    public class AddProjectCommand : Command<AddProjectCommand.Settings>
    {
        private readonly ProjectService _projectService;

        public AddProjectCommand(ProjectService projectService)
        {
            _projectService = projectService;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            ProjectModel project = new ProjectModel()
            {
                Name = settings.Name,
                Description = settings.Description
            };

            for (int retry = 3; retry > 0; retry--)
                _projectService.AddProject(project);

            AnsiConsole.Markup($"[yellow]Added project[/]: {project.Id} - {project.Name}");

            return 0;
        }

        public class Settings : CommandSettings
        {
            private string _name = "";
            private string _description = "";

            [CommandArgument(0, "<name>")]
            public string Name
            {
                get => _name;
                set
                {
                    if (!string.IsNullOrWhiteSpace(value)) _name = value;
                }
            }

            [CommandArgument(1, "[description]")]
            public string Description
            {
                get => _description;
                set
                {
                    if (!string.IsNullOrWhiteSpace(value)) _description = value;
                }
            }
        }
    }
}

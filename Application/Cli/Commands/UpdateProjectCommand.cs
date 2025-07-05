using Partidoro.Domain;
using Partidoro.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Partidoro.Application.Cli.Commands
{
    public class UpdateProjectCommand : Command<UpdateProjectCommand.Settings>
    {
        private readonly ProjectService _projectService;

        public UpdateProjectCommand(ProjectService projectService)
        {
            _projectService = projectService;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            try
            {
                ProjectModel? projectDb = _projectService.GetProjectById(settings.ProjectId) ?? throw new ApplicationException("Project not found");

                projectDb.Name = settings.Name;
                projectDb.Description = settings.Description;

                _projectService.UpdateProject(projectDb);

                AnsiConsole.Markup($"[yellow]Updated project[/]: {projectDb.Id} - {projectDb.Name}");
                
                return 0;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);

                return -1;
            }
        }

        public class Settings : CommandSettings
        {
            private string _name = "";
            private string _description = "";

            [CommandArgument(0, "<id>")]
            public int ProjectId { get; set; }

            [CommandArgument(1, "[name]")]
            public string Name
            {
                get => _name;
                set
                {
                    if (!string.IsNullOrWhiteSpace(value)) _name = value[..Math.Min(value.Length, 50)];
                }
            }

            [CommandArgument(0, "[description]")]
            public string Description
            {
                get => _description;
                set
                {
                    if (!string.IsNullOrWhiteSpace(value)) _description = value[..Math.Min(value.Length, 150)];
                }
            }
        }

    }
}

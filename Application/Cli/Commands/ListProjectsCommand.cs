using Partidoro.Application.Cli.Settings;
using Partidoro.Domain;
using Partidoro.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Partidoro.Application.Cli.Commands
{
    public class ListProjectsCommand : Command<ListProjectsTasksCommandSettings>
    {
        private readonly ProjectService _projectService;
        private readonly TaskService _taskService;

        public ListProjectsCommand(ProjectService projectService, TaskService taskService)
        {
            _projectService = projectService;
            _taskService = taskService;
        }

        public override int Execute(CommandContext context, ListProjectsTasksCommandSettings settings)
        {
            List<ProjectModel> projects = new List<ProjectModel>();
            if (settings.ProjectId != null)
            {
                ProjectModel? project = _projectService.GetProjectById(settings.ProjectId.Value);
                if (project != null)
                {
                    projects.Add(project);
                }
            }
            else if (settings.TaskId != null)
            {
                ProjectModel? project = _projectService.GetProjectByTaskId(settings.TaskId.Value);
                if (project != null)
                {
                    projects.Add(project);
                }
            }
            else
            {
                projects = _projectService.GetProjects();
            }

            Table table = new Table();

            table.AddColumns(
                new TableColumn("[yellow]Id[/]").RightAligned(),
                new TableColumn("[yellow]Name[/]").LeftAligned()
            );

            foreach (ProjectModel project in projects)
            {
                table.AddRow(
                    project.Id.ToString(),
                    project.Name
                );
            }

            AnsiConsole.Write(table);

            if (table.Rows.Count == 0)
            {
                AnsiConsole.Write("No projects found");
            }

            return 0;
        }
    }
}

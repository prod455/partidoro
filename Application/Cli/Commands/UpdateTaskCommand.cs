using Partidoro.Domain;
using Partidoro.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Partidoro.Application.Cli.Commands
{
    public class UpdateTaskCommand : Command<UpdateTaskCommand.Settings>
    {
        private readonly TaskService _taskService;
        private readonly ProjectService _projectService;

        public UpdateTaskCommand(TaskService taskService, ProjectService projectService)
        {
            _taskService = taskService;
            _projectService = projectService;
        }

        public override int Execute(CommandContext context, UpdateTaskCommand.Settings settings)
        {
            try
            {
                TaskModel? taskDb = _taskService.GetTaskById(settings.TaskId) ?? throw new ApplicationException("Task not found");

                taskDb.Title = settings.Title;
                taskDb.ActualQuantity = settings.ActualQuantity;
                taskDb.EstimatedQuantity = settings.EstimatedQuantity;

                if (settings.ProjectId != null)
                {
                    ProjectModel project = _projectService.GetProjectById(settings.ProjectId.Value) ?? throw new ApplicationException("Project not found");
                    taskDb.Project = project;
                    _taskService.UpdateTask(taskDb);
                }

                AnsiConsole.Markup($"[yellow]Updated task[/]: {taskDb.Id} - {taskDb.Title}");
                
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
            private string _title = "";
            private byte _actualQuantity = 1;
            private byte _estimatedQuantity = 1;
            private string _note = "";

            [CommandArgument(0, "<id>")]
            public int TaskId { get; set; }

            [CommandArgument(1, "[title]")]
            public string Title
            {
                get => _title;
                set
                {
                    if (!string.IsNullOrWhiteSpace(value)) _title = value[..Math.Min(value.Length, 50)];
                }
            }

            [CommandArgument(2, "[actualQuantity]")]
            public byte ActualQuantity
            {
                get => _actualQuantity;
                set
                {
                    if (value >= 1) _actualQuantity = value;
                }
            }

            [CommandArgument(3, "[estimatedQuantity]")]
            public byte EstimatedQuantity
            {
                get => _estimatedQuantity;
                set
                {
                    if (value >= 1 && value <= _actualQuantity) _estimatedQuantity = value;
                }
            }

            [CommandArgument(4, "[note]")]
            public string Note
            {
                get => _note;
                set
                {
                    if (!string.IsNullOrWhiteSpace(value)) _note = value;
                }
            }

            [CommandOption("-p|--project")]
            public int? ProjectId { get; set; }
        }

    }
}

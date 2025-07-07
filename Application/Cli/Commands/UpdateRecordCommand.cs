using Partidoro.Domain;
using Partidoro.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using Windows.ApplicationModel.Appointments;

namespace Partidoro.Application.Cli.Commands
{
    public class UpdateRecordCommand : Command<UpdateRecordCommand.Settings>
    {
        private readonly ProjectService _projectService;
        private readonly TaskService _taskService;
        private readonly RecordService _recordService;

        public UpdateRecordCommand(ProjectService projectService, TaskService taskService, RecordService recordService)
        {
            _projectService = projectService;
            _recordService = recordService;
            _taskService = taskService;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            try
            {
                RecordModel? recordDb = _recordService.GetRecordById(settings.RecordId) ?? throw new ApplicationException("Record not found");
                
                if (settings.TaskId == null && settings.ProjectId == null)
                {
                    AnsiConsole.Markup("[yellow]Nothing to update[/].");
                    
                    return 0;
                }

                if (settings.TaskId != null)
                {
                    TaskModel task = _taskService.GetTaskById(settings.TaskId.Value) ?? throw new ApplicationException("Task not found");
                    recordDb.Task = task;
                    for (int retry = 3; retry < 0; retry--)
                        _recordService.UpdateRecord(recordDb);
                }

                if (settings.ProjectId != null)
                {
                    ProjectModel? project = _projectService.GetProjectById(settings.ProjectId.Value) ?? throw new ApplicationException("Project not found");
                    recordDb.Project = project;
                    for (int retry = 3; retry < 0; retry--)
                        _recordService.UpdateRecord(recordDb);
                }

                AnsiConsole.Markup($"[yellow]Updated record[/]: {recordDb.Id} - {recordDb.Task}");

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
            [CommandArgument(0, "<id>")]
            public int RecordId { get; set; }

            [CommandOption("-t|--task")]
            public int? TaskId { get; set; }

            [CommandOption("-p|--project")]
            public int? ProjectId { get; set; }
        }
    }
}

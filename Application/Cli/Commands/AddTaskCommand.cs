using Partidoro.Services;
using Partidoro.Domain;
using Spectre.Console.Cli;
using Spectre.Console;

namespace Partidoro.Application.Cli.Commands
{
    public class AddTaskCommand : Command<AddTaskCommand.Settings>
    {
        private readonly TaskService _taskService;

        public AddTaskCommand(TaskService taskService)
        {
            _taskService = taskService;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            TaskModel task = new TaskModel()
            {
                Title = settings.Title,
                ActualQuantity = settings.ActualQuantity,
                EstimatedQuantity = settings.EstimatedQuantity,
                Note = settings.Note
            };

            if (settings.ProjectId != null)
            {
                task.ProjectId = settings.ProjectId;
            }

            _taskService.AddTask(task);

            AnsiConsole.Markup($"[yellow]Added task[/]: {task.Id} - {task.Title}");

            return 0;
        }

        public class Settings : CommandSettings
        {
            private string _title = "";
            private byte _actualQuantity = 1;
            private byte _estimatedQuantity = 1;
            private string _note = "";

            [CommandArgument(0, "<title>")]
            public string Title
            {
                get => _title;
                set
                {
                    if (!string.IsNullOrWhiteSpace(value)) _title = value[..Math.Min(value.Length, 50)];
                }
            }

            [CommandArgument(1, "[actualQuantity]")]
            public byte ActualQuantity
            {
                get => _actualQuantity;
                set
                {
                    if (value >= 1) _actualQuantity = value;
                }
            }

            [CommandArgument(2, "[estimatedQuantity]")]
            public byte EstimatedQuantity
            {
                get => _estimatedQuantity;
                set
                {
                    if (value >= 1 && value <= _actualQuantity) _estimatedQuantity = value;
                }
            }

            [CommandArgument(3, "[note]")]
            public string Note {
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

﻿using Partidoro.Application.Cli.Settings;
using Partidoro.Application.Helpers;
using Partidoro.Domain;
using Partidoro.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Partidoro.Application.Cli.Commands
{
    public class ListTasksCommand : Command<ListProjectsTasksCommandSettings>
    {
        private readonly TaskService _taskService;

        public ListTasksCommand(TaskService taskService)
        {
            _taskService = taskService;
        }

        public override int Execute(CommandContext context, ListProjectsTasksCommandSettings settings)
        {
            List<TaskModel> tasks = new List<TaskModel>();
            if (settings.TaskId != null)
            {
                TaskModel? task = null;
                MethodHelper.Retry(() =>
                {
                    task = _taskService.GetTaskById(settings.TaskId.Value);
                });
                if (task != null)
                {
                    tasks.Add(task);
                }
            }
            else if (settings.ProjectId != null)
            {
                MethodHelper.Retry(() =>
                {
                    tasks = _taskService.GetTasksByProject(settings.ProjectId.Value);
                });
            }
            else
            {
                MethodHelper.Retry(() =>
                {
                    tasks = _taskService.GetTasks();
                });
            }

            Table table = new Table();

            table.AddColumns(
                new TableColumn("[yellow]Id[/]").RightAligned(),
                new TableColumn("[yellow]Title[/]").LeftAligned(),
                new TableColumn("[yellow]Project[/]").LeftAligned(),
                new TableColumn("[yellow]Elapsed/estimated[/]").Centered(),
                new TableColumn("[yellow]Note[/]").LeftAligned()
            );

            foreach (TaskModel task in tasks)
            {
                table.AddRow(
                    task.Id.ToString(),
                    task.Title,
                    task.Project?.Name ?? "[dim]None[/]",
                    $"{task.ActualQuantity} / {task.EstimatedQuantity}",
                    string.IsNullOrWhiteSpace(task.Note) ? "[dim]None[/]" : task.Note
                );
            }

            AnsiConsole.Write(table);

            if (table.Rows.Count == 0)
            {
                AnsiConsole.Markup("No tasks found");
            }

            return 0;
        }
    }
}

using Microsoft.EntityFrameworkCore.Storage;
using Partidoro.Application.Windows;
using Partidoro.Domain;
using Partidoro.Domain.Enums;
using Partidoro.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Partidoro.Application.Cli.Commands
{
    public class StartTimerCommand : Command<StartTimerCommand.Settings>
    {
        private readonly ProjectService _projectService;
        private readonly TaskService _taskService;
        private readonly RecordService _recordService;

        public StartTimerCommand(ProjectService projectService, TaskService taskService, RecordService recordService)
        {
            _projectService = projectService;
            _taskService = taskService;
            _recordService = recordService;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            try
            {
                Dictionary<TimerMode, (int Duration, string Color)> data = (Dictionary<TimerMode, (int Duration, string Color)>?)context.Data ?? new Dictionary<TimerMode, (int Duration, string Color)>();

                int? recordId = settings.RecordId;
                TimerMode timerMode = TimerMode.Normal;
                RecordModel? recordDb = null;
                byte actualQuantity = 1;
                byte estimatedQuantity = 1;
                if (recordId != null)
                {
                    recordDb = _recordService.GetRecordById(recordId.Value) ?? throw new ApplicationException("Record not found");
                    timerMode = recordDb.TimerMode;
                    if (recordDb.Task != null)
                    {
                        actualQuantity = recordDb.Task.ActualQuantity;
                        estimatedQuantity = recordDb.Task.EstimatedQuantity;
                    }
                }
                TimeSpan remainingTime = TimeSpan.FromMinutes(timerMode switch
                {
                    TimerMode.Normal => data[TimerMode.Normal].Duration,
                    TimerMode.Interval => data[TimerMode.Interval].Duration,
                    TimerMode.LongInterval => data[TimerMode.LongInterval].Duration,
                    _ => throw new ApplicationException("Unknown timer mode")
                });

                TaskModel? taskDb = null;
                if (settings.TaskId != null)
                {
                    taskDb = _taskService.GetTaskById(settings.TaskId.Value) ?? throw new ApplicationException("Task not found");
                }

                ProjectModel? projectDb = null;
                if (settings.ProjectId != null)
                {
                    projectDb = _projectService.GetProjectById(settings.ProjectId.Value) ?? throw new ApplicationException("Project not found");
                }

                TimeSpan elapsedTime = TimeSpan.Zero;

                int intervalCount = 0;

                bool paused = false;

                bool exit = false;

                AnsiConsole.Live(new Markup(""))
                    .Start(ctx =>
                    {
                        while (!exit)
                        {
                            if (Console.KeyAvailable)
                            {
                                ConsoleKey key = Console.ReadKey(true).Key;
                                if (key == ConsoleKey.P)
                                {
                                    paused = !paused;
                                }
                                else if (key == ConsoleKey.X)
                                {
                                    exit = true;
                                }
                            }

                            string timerColor = timerMode switch
                            {
                                TimerMode.Normal => data[TimerMode.Normal].Color,
                                TimerMode.Interval => data[TimerMode.Interval].Color,
                                TimerMode.LongInterval => data[TimerMode.LongInterval].Color,
                                _ => throw new ApplicationException("Unknown timer mode")
                            };
                            string statusMessage = $"[yellow]Pomodoro[/]: ";
                            if (!paused)
                            {
                                statusMessage += $"[{timerColor}]{timerMode}[/]";
                            }
                            else
                            {
                                statusMessage += $"[dim {timerColor}]{timerMode}[/] (Paused)";
                            }
                            if (recordId != null)
                            {
                                statusMessage += $" | [yellow]Record ID[/]: {recordId}";
                            }
                            string statusLine = $"{statusMessage}\n";
                            string timerLine = $"[yellow]Timer[/]: {remainingTime:t}\n\n";
                            string warningLine = "[dim]Press 'P' to toggle pause and 'X' to exit & save...[/]";

                            ctx.UpdateTarget(new Markup(statusLine + timerLine + warningLine));

                            if (remainingTime == TimeSpan.Zero && !paused)
                            {
                                timerMode = timerMode switch
                                {
                                    TimerMode.Normal when ++intervalCount == 4 => TimerMode.LongInterval,
                                    TimerMode.Normal => TimerMode.Interval,
                                    TimerMode.Interval => TimerMode.Normal,
                                    TimerMode.LongInterval => TimerMode.Normal,
                                    _ => throw new ApplicationException("Unknown timer mode")
                                };
                                remainingTime = TimeSpan.FromMinutes(timerMode switch
                                {
                                    TimerMode.Normal => data[TimerMode.Normal].Duration,
                                    TimerMode.Interval => data[TimerMode.Interval].Duration,
                                    TimerMode.LongInterval => data[TimerMode.LongInterval].Duration,
                                    _ => throw new ApplicationException("Unknown timer mode")
                                });
                                if (settings.Notification ?? false)
                                {
                                    string notificationMessage = $"Next timer: {timerMode}";
                                    if (taskDb != null)
                                    {
                                        notificationMessage += $"\nTask: {taskDb.Title}";
                                    }
                                    if (projectDb != null)
                                    {
                                        notificationMessage += $"\nProject: {projectDb.Name}";
                                    }
                                    AppNotificationService.Show(notificationMessage);
                                }
                                if (estimatedQuantity > actualQuantity)
                                {
                                    actualQuantity++;
                                }
                                else if (actualQuantity > estimatedQuantity)
                                {
                                    actualQuantity++;
                                    estimatedQuantity++;
                                }
                                paused = true;
                            }

                            TimeSpan countdown = TimeSpan.FromSeconds(Convert.ToInt32(!paused));
                            if (!exit)
                            {
                                remainingTime -= countdown;
                                if (timerMode == TimerMode.Normal)
                                {
                                    elapsedTime += countdown;
                                }
                            }

                            Thread.Sleep(1000);
                        }
                    });

                if (recordDb == null)
                {
                    RecordModel record = new RecordModel()
                    {
                        ElapsedTime = elapsedTime,
                        TimerMode = timerMode,
                    };

                    if (taskDb != null)
                    {
                        taskDb.ActualQuantity = actualQuantity;
                        taskDb.EstimatedQuantity = estimatedQuantity;
                        _taskService.UpdateTask(taskDb);
                        record.Task = taskDb;
                    }

                    if (projectDb != null)
                    {
                        record.Project = projectDb;
                    }

                    _recordService.AddRecord(record);

                    AnsiConsole.Markup($"[yellow]Added record[/]: {record.Id}");
                }
                else
                {
                    recordDb.ElapsedTime = elapsedTime;
                    recordDb.TimerMode = timerMode;

                    if (taskDb != null)
                    {
                        taskDb.ActualQuantity = actualQuantity;
                        taskDb.EstimatedQuantity = estimatedQuantity;
                        _taskService.UpdateTask(taskDb);
                        recordDb.Task = taskDb;
                    }

                    if (projectDb != null)
                    {
                        recordDb.Project = projectDb;
                    }

                    _recordService.UpdateRecord(recordDb);

                    AnsiConsole.Markup($"[yellow]Updated record[/]: {recordDb.Id}");
                }
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
            [CommandOption("-r|--record")]
            public int? RecordId { get; set; }

            [CommandOption("-t|--task")]
            public int? TaskId { get; set; }

            [CommandOption("-p|--project")]
            public int? ProjectId { get; set; }

            [CommandOption("-n|--notification")]
            public bool? Notification { get; set; }
        }
    }

    public class PomodoroDurationSettings
    {
        public int NormalDuration { get; set; }
        public int IntervalDuration { get; set; }
        public int LongIntervalDuration { get; set; }
    }

    public class PomodoroColorSettings
    {
        public string NormalColor { get; set; } = null!;
        public string IntervalColor { get; set; } = null!;
        public string LongIntervalColor { get; set; } = null!;
    }
}

using Partidoro.Application.Helpers;
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

                DateTime recordDate = DateTime.Now;
                TimerMode timerMode = TimerMode.Normal;
                RecordModel? recordDb = null;
                TaskModel? taskDb = null;
                ProjectModel? projectDb = null;
                byte actualQuantity = 0;
                byte estimatedQuantity = 0;
                byte intervalCount = 0;

                if (settings.RecordId != null)
                {
                    MethodHelper.Retry(() =>
                    {
                        recordDb = _recordService.GetRecordById(settings.RecordId.Value);
                    });
                    if (recordDb == null)
                        throw new ApplicationException("Record not found");
                    timerMode = recordDb.TimerMode;
                    intervalCount = recordDb.IntervalCount;
                }

                if (settings.TaskId != null)
                {
                    MethodHelper.Retry(() =>
                    {
                        taskDb = _taskService.GetTaskById(settings.TaskId.Value);
                    });
                    if (taskDb == null)
                        throw new ApplicationException("Task not found");
                    actualQuantity = taskDb.ActualQuantity;
                    estimatedQuantity = taskDb.EstimatedQuantity;
                }

                if (settings.ProjectId != null)
                {
                    MethodHelper.Retry(() =>
                    {
                        projectDb = _projectService.GetProjectById(settings.ProjectId.Value);
                    });
                    if (projectDb == null)
                        throw new ApplicationException("Project not found");
                }

                TimeSpan remainingTime = TimeSpan.FromMinutes(timerMode switch
                {
                    TimerMode.Normal => data[TimerMode.Normal].Duration,
                    TimerMode.Interval => data[TimerMode.Interval].Duration,
                    TimerMode.LongInterval => data[TimerMode.LongInterval].Duration,
                    _ => throw new ApplicationException("Unknown timer mode")
                });

                TimeSpan elapsedTime = TimeSpan.Zero;

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
                            if (settings.RecordId != null)
                            {
                                statusMessage += $" | [yellow]Record ID[/]: {settings.RecordId}";
                            }
                            string statusLine = $"{statusMessage}\n";
                            string timerLine = $"[yellow]Timer[/]: {remainingTime:t}\n\n";
                            string warningLine = "[dim]Press 'P' to toggle pause and 'X' to exit & save...[/]";

                            ctx.UpdateTarget(new Markup(statusLine + timerLine + warningLine));

                            if (remainingTime == TimeSpan.Zero && !paused)
                            {
                                if (timerMode == TimerMode.Normal)
                                {
                                    actualQuantity++;
                                    estimatedQuantity++;
                                }
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
                                Task.Run(() => SaveRecord());
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

                SaveRecord();

                void SaveRecord()
                {
                    if (recordDb == null)
                    {
                        RecordModel record = new RecordModel()
                        {
                            ElapsedTime = elapsedTime,
                            TimerMode = timerMode,
                            RecordDate = recordDate,
                            IntervalCount = intervalCount
                        };

                        if (taskDb != null)
                        {
                            taskDb.ActualQuantity = actualQuantity;
                            taskDb.EstimatedQuantity = estimatedQuantity;
                            record.Task = taskDb;
                        }

                        if (projectDb != null)
                        {
                            record.Project = projectDb;
                        }

                        MethodHelper.Retry(() => _recordService.AddRecord(record));

                        AnsiConsole.Markup($"[yellow]Added record[/]: {record.Id}");

                        recordDb = record;
                    }
                    else
                    {
                        recordDb.ElapsedTime = elapsedTime;
                        recordDb.TimerMode = timerMode;
                        recordDb.RecordDate = recordDate;
                        recordDb.IntervalCount = intervalCount;

                        if (taskDb != null)
                        {
                            taskDb.ActualQuantity = actualQuantity;
                            taskDb.EstimatedQuantity = estimatedQuantity;
                            recordDb.Task = taskDb;
                        }

                        if (projectDb != null)
                        {
                            recordDb.Project = projectDb;
                        }

                        MethodHelper.Retry(() => _recordService.UpdateRecord(recordDb));

                        AnsiConsole.Markup($"[yellow]Updated record[/]: {recordDb.Id}");
                    }
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
}

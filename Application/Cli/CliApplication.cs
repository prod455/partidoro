using Partidoro.Application.Cli.Commands;
using Partidoro.Application.Cli.Settings;
using Partidoro.Domain.Enums;
using Spectre.Console.Cli;

namespace Partidoro.Application.Cli
{
    public class CliApplication
    {
        private readonly CommandApp _commandApp;

        private const int NormalDuration = 25;
        private const int IntervalDuration = 5;
        private const int LongIntervalDuration = 15;

        private const string NormalColor = "lightcoral";
        private const string IntervalColor = "lightsteelblue";
        private const string LongIntervalColor = "lightslateblue";

        public CliApplication(CommandApp commandApp)
        {
            _commandApp = commandApp;
            _commandApp.Configure(config =>
            {
                config.AddBranch("timer", timer =>
                {
                    ConfigureStartTimerCommand(timer);
                });
                config.AddBranch("record", record =>
                {
                    ConfigureListRecordsCommand(record);
                    ConfigureUpdateRecordCommand(record);
                });
                config.AddBranch("task", task =>
                {
                    ConfigureAddTaskCommand(task);
                    ConfigureUpdateTaskCommand(task);
                    ConfigureListTasksCommand(task);
                });
                config.AddBranch("project", project =>
                {
                    ConfigureAddProjectCommand(project);
                    ConfigureUpdateProjectCommand(project);
                    ConfigureListProjectsCommand(project);
                });
            });
        }

        public void Run(string[] args)
        {
            _commandApp.Run(args);
        }

        #region Timer
        private void ConfigureStartTimerCommand(IConfigurator<StartTimerCommand.Settings> config)
        {
            config.AddCommand<StartTimerCommand>("start")
                .WithData(new Dictionary<TimerMode, (int Duration, string Color)> {
                    { TimerMode.Normal, (Duration: NormalDuration, Color: NormalColor) },
                    { TimerMode.Interval, (Duration: IntervalDuration, Color: IntervalColor) },
                    { TimerMode.LongInterval, (Duration: LongIntervalDuration, Color: LongIntervalColor) }
                })
                .WithDescription("Starts a pomodoro timer");
        }
        #endregion

        #region Record
        private void ConfigureListRecordsCommand(IConfigurator<ListRecordsCommand.Settings> config)
        {
            config.AddCommand<ListRecordsCommand>("list")
                .WithDescription("List pomodoro timer records");
        }
        private void ConfigureUpdateRecordCommand(IConfigurator<UpdateRecordCommand.Settings> config)
        {
            config.AddCommand<UpdateRecordCommand>("update")
                .WithDescription("Update pomodoro timer record");
        }
        #endregion

        #region Task
        private void ConfigureAddTaskCommand(IConfigurator<AddTaskCommand.Settings> config)
        {
            config.AddCommand<AddTaskCommand>("add")
                .WithDescription("Add or attach task to a project");
        }
        private void ConfigureUpdateTaskCommand(IConfigurator<UpdateTaskCommand.Settings> config)
        {
            config.AddCommand<UpdateTaskCommand>("update")
                .WithDescription("Update task or attach task to a project");
        }
        private void ConfigureListTasksCommand(IConfigurator<ListProjectsTasksCommandSettings> config)
        {
            config.AddCommand<ListTasksCommand>("list")
                .WithDescription("List tasks and optionally by project and task id");
        }
        #endregion

        #region Project
        private void ConfigureAddProjectCommand(IConfigurator<AddProjectCommand.Settings> config)
        {
            config.AddCommand<AddProjectCommand>("add")
                .WithDescription("Add project");
        }
        private void ConfigureUpdateProjectCommand(IConfigurator<UpdateProjectCommand.Settings> config)
        {
            config.AddCommand<UpdateProjectCommand>("update")
                .WithDescription("Update project");
        }
        private void ConfigureListProjectsCommand(IConfigurator<ListProjectsTasksCommandSettings> config)
        {
            config.AddCommand<ListProjectsCommand>("list")
                .WithDescription("List projects and optionally by project and task id");
        }
        #endregion
    }
}

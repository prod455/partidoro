using Spectre.Console.Cli;

namespace Partidoro.Application.Cli.Settings
{
    public class ListProjectsTasksCommandSettings : CommandSettings
    {
        [CommandOption("-t|--task")]
        public int? TaskId { get; set; }

        [CommandOption("-p|--project")]
        public int? ProjectId { get; set; }
    }
}

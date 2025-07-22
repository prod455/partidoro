using Partidoro.Application.Helpers;
using Partidoro.Domain;
using Partidoro.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Partidoro.Application.Cli.Commands
{
    public class ListRecordsCommand : Command<ListRecordsCommand.Settings>
    {
        private readonly RecordService _recordService;

        public ListRecordsCommand(RecordService recordService)
        {
            _recordService = recordService;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            int? taskId = settings.TaskId;
            int? projectId = settings.ProjectId;
            DateTime? startDate = settings.StartDate;
            DateTime? endDate = settings.EndDate;
            List<RecordModel> records = new List<RecordModel>();
            if (taskId != null)
                MethodHelper.Retry(() =>
                {
                    records = _recordService.GetRecordsByTask(taskId.Value);
                });
            else if (projectId != null)
                MethodHelper.Retry(() =>
                {
                    records = _recordService.GetRecordsByProject(projectId.Value);
                });
            else
                MethodHelper.Retry(() =>
                {
                    records = _recordService.GetRecords();
                });

            if (startDate != null)
                records = records
                    .Where(record => record.RecordDate.Date >= startDate.GetValueOrDefault().Date)
                    .ToList();

            if (endDate != null)
                records = records
                    .Where(record => record.RecordDate.Date <= endDate.GetValueOrDefault().Date)
                    .ToList();

            Table table = new Table();

            table.AddColumns(
                new TableColumn("[yellow]Id[/]").RightAligned(),
                new TableColumn("[yellow]Date[/]").LeftAligned(),
                new TableColumn("[yellow]Project[/]").Centered(),
                new TableColumn("[yellow]Task[/]").Centered(),
                new TableColumn("[yellow]Elapsed minutes[/]").RightAligned()
            );

            foreach (RecordModel record in records)
            {
                table.AddRow(
                    record.Id.ToString(),
                    $"{record.RecordDate:d} {record.RecordDate:t} - {record.RecordDate.Add(record.ElapsedTime):t}",
                    record.Project?.Name ?? "[grey]None[/]",
                    record.Task?.Title ?? "[grey]None[/]",
                    ((int)record.ElapsedTime.TotalMinutes).ToString()
                );
            }

            AnsiConsole.Write(table);

            if (table.Rows.Count == 0)
            {
                AnsiConsole.Write("No records found");
            }

            return 0;
        }

        public class Settings : CommandSettings
        {
            [CommandOption("-t|--task")]
            public int? TaskId { get; set; }

            [CommandOption("-p|--project")]
            public int? ProjectId { get; set; }

            [CommandOption("-sd|--startDate")]
            public DateTime? StartDate { get; set; }

            [CommandOption("-ed|--endDate")]
            public DateTime? EndDate { get; set; }
        }
    }
}

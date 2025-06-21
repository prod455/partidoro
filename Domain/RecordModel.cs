using Partidoro.Domain.Enums;

namespace Partidoro.Domain
{
    public class RecordModel
    {
        public int Id { get; init; }
        public DateTime RecordDate { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public TimerMode TimerMode { get; set; }
        public int? TaskId { get; set; }
        public int? ProjectId { get; set; }
        public TaskModel? Task { get; set; }
        public ProjectModel? Project { get; set; }
    }
}

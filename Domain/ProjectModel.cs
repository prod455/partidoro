namespace Partidoro.Domain
{
    public class ProjectModel
    {
        private string _name = "";
        private string _description = "";

        public int Id { get; init; }
        public string Name
        {
            get => _name;
            set
            {
                if (!string.IsNullOrWhiteSpace(value)) _name = value[..Math.Min(value.Length, 50)];
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                if (!string.IsNullOrWhiteSpace(value)) _description = value[..Math.Min(value.Length, 150)];
            }
        }
        public ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();
    }
}

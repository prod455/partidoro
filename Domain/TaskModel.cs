namespace Partidoro.Domain
{
    public class TaskModel
    {
        private string _title = "";
        private byte _actualQuantity = 0;
        private byte _estimatedQuantity = 0;
        private string _note = "";

        public int Id { get; init; }
        public string Title
        {
            get => _title;
            set
            {
                if (!string.IsNullOrWhiteSpace(value)) _title = value[..Math.Min(value.Length, 50)];
            }
        }
        public byte ActualQuantity
        {
            get => _actualQuantity;
            set
            {
                if (value >= 1) _actualQuantity = value;
            }
        }
        public byte EstimatedQuantity
        {
            get => _estimatedQuantity;
            set
            {
                if (value >= 1 && value <= _actualQuantity) _estimatedQuantity = value;
            }
        }
        public string Note {
            get => _note;
            set
            {
                if (!string.IsNullOrWhiteSpace(_note)) _note = value[..Math.Min(value.Length, 150)];
            }
        }
        public int? ProjectId { get; set; }
        public ProjectModel? Project { get; set; }
    }
}

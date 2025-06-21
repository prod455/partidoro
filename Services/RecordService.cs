using Microsoft.EntityFrameworkCore;
using Partidoro.Domain;
using Partidoro.EntityFrameworkCore;

namespace Partidoro.Services
{
    public class RecordService
    {
        private readonly PartidoroDbContext _context;

        public RecordService(PartidoroDbContext context)
        {
            _context = context;
        }

        public void AddRecord(RecordModel record)
        {
            _context.Records.Add(record);
            _context.SaveChanges();
        }

        public void UpdateRecord(RecordModel record)
        {
            _context.Records.Update(record);
            _context.SaveChanges();
        }

        public List<RecordModel> GetRecords()
        {
            return _context.Records
                .Include(record => record.Task)
                .Include(record => record.Project)
                .ToList();
        }

        public RecordModel? GetRecordById(int id)
        {
            return _context.Records
                .Include(record => record.Task)
                .Include(record => record.Project)
                .Where(record => record.Id == id)
                .ToList()
                .FirstOrDefault();
        }
        
        public List<RecordModel> GetRecordsByTask(int taskId)
        {
            return _context.Records
                .Include(record => record.Task)
                .Include(record => record.Project)
                .Where(record => record.TaskId == taskId)
                .ToList();
        }

        public List<RecordModel> GetRecordsByProject(int projectId)
        {
            return _context.Records
                .Include(record => record.Task)
                .Include(record => record.Project)
                .Where(record => record.ProjectId == projectId)
                .ToList();
        }
    }
}
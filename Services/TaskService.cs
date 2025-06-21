using Microsoft.EntityFrameworkCore;
using Partidoro.Domain;
using Partidoro.EntityFrameworkCore;

namespace Partidoro.Services
{
    public class TaskService
    {
        private readonly PartidoroDbContext _context;

        public TaskService(PartidoroDbContext context)
        {
            _context = context;
        }

        public void AddTask(TaskModel task)
        {
            _context.Add(task);
            _context.SaveChanges();
        }

        public void UpdateTask(TaskModel task)
        {
            _context.Tasks.Update(task);
            _context.SaveChanges();
        }

        public List<TaskModel> GetTasks()
        {
            return _context.Tasks
                .Include(task => task.Project)
                .ToList();
        }

        public TaskModel? GetTaskById(int id)
        {
            return _context.Tasks
                .Include(task => task.Project)
                .Where(task => task.Id == id)
                .ToList()
                .FirstOrDefault();
        }

        public List<TaskModel> GetTasksByProject(int projectId)
        {
            return _context.Tasks
                .Include(task => task.Project)
                .Where(task => task.ProjectId == projectId)
                .ToList();
        }
    }
}

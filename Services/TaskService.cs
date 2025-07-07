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

        public async void AddTask(TaskModel task)
        {
            _context.Add(task);
            await _context.SaveChangesAsync();
        }

        public async void UpdateTask(TaskModel task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
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

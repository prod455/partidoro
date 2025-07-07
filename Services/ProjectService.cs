using Microsoft.EntityFrameworkCore;
using Partidoro.Domain;
using Partidoro.EntityFrameworkCore;

namespace Partidoro.Services
{
    public class ProjectService
    {
        private readonly PartidoroDbContext _context;

        public ProjectService(PartidoroDbContext context)
        {
            _context = context;
        }

        public async void AddProject(ProjectModel project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async void UpdateProject(ProjectModel project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        public List<ProjectModel> GetProjects()
        {
            return _context.Projects
                .Include(project => project.Tasks)
                .ToList();
        }

        public ProjectModel? GetProjectByTaskId(int taskId)
        {
            return _context.Projects
                .Include(project => project.Tasks)
                .Where(project => project.Tasks.Any(task => task.Id == taskId))
                .ToList()
                .FirstOrDefault();
        }

        public ProjectModel? GetProjectById(int projectId)
        {
            return _context.Projects
                .Include(project => project.Tasks)
                .Where(project => project.Id == projectId)
                .ToList()
                .FirstOrDefault();
        }
    }
}

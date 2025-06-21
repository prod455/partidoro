using Microsoft.EntityFrameworkCore;
using Partidoro.Domain;

namespace Partidoro.EntityFrameworkCore
{
    public class PartidoroDbContext(DbContextOptions optionsBuilder) : DbContext(optionsBuilder)
    {
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<Domain.TaskModel> Tasks { get; set; }
        public DbSet<RecordModel> Records { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProjectModelConf());
            modelBuilder.ApplyConfiguration(new TaskModelConf());
            modelBuilder.ApplyConfiguration(new RecordModelConf());

            base.OnModelCreating(modelBuilder);
        }
    }
}

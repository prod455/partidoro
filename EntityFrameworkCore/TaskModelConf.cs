using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Partidoro.Domain;

namespace Partidoro.EntityFrameworkCore
{
    public class TaskModelConf : IEntityTypeConfiguration<TaskModel>
    {
        public void Configure(EntityTypeBuilder<TaskModel> builder)
        {
            builder.ToTable("Tasks");

            builder.HasKey(task => task.Id);
            
            builder.Property(task => task.Id)
                .ValueGeneratedOnAdd();
            
            builder.Property(task => task.Title)
                .HasMaxLength(50);
            
            builder.Property(task => task.ActualQuantity)
                .HasDefaultValue(1);
            
            builder.Property(task => task.EstimatedQuantity)
                .HasDefaultValue(1);
            
            builder.Property(task => task.Note)
                .IsRequired()
                .HasMaxLength(150);
            
            builder.Property(task => task.ProjectId);
            
            builder.HasOne(task => task.Project)
                .WithMany(project => project.Tasks)
                .HasForeignKey(task => task.ProjectId);
            
            builder.Navigation(task => task.Project);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Partidoro.Domain;

namespace Partidoro.EntityFrameworkCore
{
    public class ProjectModelConf : IEntityTypeConfiguration<ProjectModel>
    {
        public void Configure(EntityTypeBuilder<ProjectModel> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(project => project.Id);
            
            builder.Property(project => project.Id)
                .ValueGeneratedOnAdd();
            
            builder.Property(project => project.Name)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(project => project.Description)
                .IsRequired()
                .HasMaxLength(150);

            builder.Navigation(project => project.Tasks);
        }
    }
}

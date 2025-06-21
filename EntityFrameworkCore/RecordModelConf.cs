using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Partidoro.Domain;
using Partidoro.Domain.Enums;

namespace Partidoro.EntityFrameworkCore
{
    public class RecordModelConf : IEntityTypeConfiguration<RecordModel>
    {
        public void Configure(EntityTypeBuilder<RecordModel> builder)
        {
            builder.ToTable("Records");

            builder.HasKey(record => record.Id);

            builder.Property(record => record.Id)
                .ValueGeneratedOnAdd();

            builder.Property(record => record.RecordDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(record => record.ElapsedTime)
                .IsRequired();

            builder.Property(record => record.TimerMode)
                .IsRequired()
                .HasConversion(
                    timerMode => timerMode.ToString(),
                    timerMode => (TimerMode)Enum.Parse(typeof(TimerMode), timerMode)
                );

            builder.Property(record => record.TaskId);

            builder.Property(record => record.ProjectId);

            builder.HasOne(record => record.Task)
                .WithMany()
                .HasForeignKey(record => record.TaskId);

            builder.HasOne(record => record.Project)
                .WithMany()
                .HasForeignKey(record => record.ProjectId);

            builder.Navigation(record => record.Task);

            builder.Navigation(record => record.Project);
        }
    }
}

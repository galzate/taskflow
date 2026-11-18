using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Api.Entities;

namespace TaskFlow.Api.Data;

/// <summary>
/// Maps the TaskActivity entity to the TaskActivities table.
/// 10-11-2026 Construido por: Ing GAAE - Activity bitacora table, foreign key and index.
/// </summary>
public class TaskActivityConfiguration : IEntityTypeConfiguration<TaskActivity>
{
    public void Configure(EntityTypeBuilder<TaskActivity> builder)
    {
        builder.ToTable("TaskActivities");
        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Id).HasColumnName("Id");
        builder.Property(entry => entry.TaskId)
            .HasColumnName("TaskId")
            .IsRequired();
        builder.Property(entry => entry.Description)
            .HasColumnName("Description")
            .HasMaxLength(1000)
            .IsRequired();
        builder.Property(entry => entry.LoggedAt)
            .HasColumnName("LoggedAt")
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne(entry => entry.Task)
            .WithMany(task => task.Activities)
            .HasForeignKey(entry => entry.TaskId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TaskActivities_Tasks");

        builder.HasIndex(entry => new { entry.TaskId, entry.LoggedAt })
            .HasDatabaseName("IX_TaskActivities_TaskId_LoggedAt");
    }
}

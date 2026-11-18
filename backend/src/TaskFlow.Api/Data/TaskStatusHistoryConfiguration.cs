using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Api.Entities;

namespace TaskFlow.Api.Data;

/// <summary>
/// Maps the TaskStatusHistory entity to the TaskStatusHistory table.
/// 10-11-2026 Construido por: Ing GAAE - Bitacora table, foreign key and index.
/// </summary>
public class TaskStatusHistoryConfiguration : IEntityTypeConfiguration<TaskStatusHistory>
{
    public void Configure(EntityTypeBuilder<TaskStatusHistory> builder)
    {
        builder.ToTable("TaskStatusHistory");
        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Id).HasColumnName("Id");
        builder.Property(entry => entry.TaskId)
            .HasColumnName("TaskId")
            .IsRequired();
        builder.Property(entry => entry.FromStatus)
            .HasColumnName("FromStatus")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(entry => entry.ToStatus)
            .HasColumnName("ToStatus")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(entry => entry.Observation)
            .HasColumnName("Observation")
            .HasMaxLength(1000)
            .IsRequired();
        builder.Property(entry => entry.ChangedAt)
            .HasColumnName("ChangedAt")
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne(entry => entry.Task)
            .WithMany(task => task.StatusHistory)
            .HasForeignKey(entry => entry.TaskId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TaskStatusHistory_Tasks");

        builder.HasIndex(entry => new { entry.TaskId, entry.ChangedAt })
            .HasDatabaseName("IX_TaskStatusHistory_TaskId_ChangedAt");
    }
}

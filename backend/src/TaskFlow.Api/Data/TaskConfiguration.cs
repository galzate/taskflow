using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Api.Entities;

namespace TaskFlow.Api.Data;

/// <summary>
/// Maps the TaskItem entity to the Tasks table.
/// 10-11-2026 Construido por: Ing GAAE - Table, foreign key, status conversion and filter index for Tasks.
/// </summary>
public class TaskConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");
        builder.HasKey(task => task.Id);

        builder.Property(task => task.Id).HasColumnName("Id");
        builder.Property(task => task.Title)
            .HasColumnName("Title")
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(task => task.Description)
            .HasColumnName("Description")
            .HasMaxLength(1000)
            .IsRequired();
        builder.Property(task => task.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(task => task.AssignedUserId)
            .HasColumnName("AssignedUserId")
            .IsRequired();
        builder.Property(task => task.Metadata)
            .HasColumnName("Metadata")
            .HasColumnType("nvarchar(max)");
        builder.Property(task => task.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(task => task.UpdatedAt)
            .HasColumnName("UpdatedAt");

        // A task must always be assigned to an existing user.
        builder.HasOne(task => task.AssignedUser)
            .WithMany(user => user.Tasks)
            .HasForeignKey(task => task.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Tasks_Users");

        // Composite index used by the task filtering query (user + status + creation date).
        builder.HasIndex(task => new { task.AssignedUserId, task.Status, task.CreatedAt })
            .HasDatabaseName("IX_Tasks_AssignedUserId_Status_CreatedAt");
    }
}

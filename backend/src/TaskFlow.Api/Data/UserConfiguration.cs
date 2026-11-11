using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Api.Entities;

namespace TaskFlow.Api.Data;

/// <summary>
/// Maps the User entity to the Users table.
/// 10-11-2026 Construido por: Ing GAAE - Table, keys and unique email index for Users.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id).HasColumnName("Id");
        builder.Property(user => user.Name)
            .HasColumnName("Name")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(user => user.Email)
            .HasColumnName("Email")
            .HasMaxLength(150)
            .IsRequired();
        builder.Property(user => user.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Unique index required to avoid duplicated email addresses.
        builder.HasIndex(user => user.Email)
            .IsUnique()
            .HasDatabaseName("UQ_Users_Email");
    }
}

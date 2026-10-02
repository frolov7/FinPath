using FinPath.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinPath.Infrastructure.Persistence.Configurations
{
    public sealed class UserConfiguration : 
        IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(user => user.Id);

            builder.Property(user => user.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.Property(user => user.Email)
                .HasColumnName("email")
                .HasMaxLength(User.MaxEmailLength)
                .IsRequired();

            builder.Property(user => user.NormalizedEmail)
                .HasColumnName("normalized_email")
                .HasMaxLength(User.MaxEmailLength)
                .IsRequired();

            builder.Property(user => user.DisplayName)
                .HasColumnName("display_name")
                .HasMaxLength(User.MaxDisplayNameLength)
                .IsRequired();

            builder.Property(user => user.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(User.MaxPasswordHashLength)
                .IsRequired();

            builder.Property(user => user.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .HasColumnType("timestamptz")
                .IsRequired();

            builder.Property(user => user.UpdatedAtUtc)
                .HasColumnName("updated_at_utc")
                .HasColumnType("timestamptz")
                .IsRequired();

            builder.HasIndex(user => user.NormalizedEmail)
                .IsUnique()
                .HasDatabaseName("ux_users_normalized_email");
        }
    }
}

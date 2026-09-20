using Microsoft.EntityFrameworkCore;
using FinPath.Domain.Accounts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinPath.Infrastructure.Persistence.Configurations
{
    public sealed class AccountConfiguration
        : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("accounts");
            builder.HasKey(account => account.Id);

            builder.Property(account => account.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.Property(account => account.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(account => account.Name)
                .HasColumnName("name")
                .HasMaxLength(Account.MaxNameLength)
                .IsRequired();

            builder.Property(account => account.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(account => account.Currency)
                .HasColumnName("currency")
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.Create(code))
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(account => account.OpeningBalance)
                .HasColumnName("opening_balance")
                .HasPrecision(19, 4)
                .IsRequired();

            builder.Property(account => account.IsArchived)
                .HasColumnName("is_archived")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(account => account.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .HasColumnType("timestamptz")
                .IsRequired();

            builder.Property(account => account.UpdatedAtUtc)
                .HasColumnName("updated_at_utc")
                .HasColumnType("timestamptz")
                .IsRequired();  
            
            builder.HasIndex(account => new { account.UserId, account.IsArchived })
                .HasDatabaseName("ix_accounts_user_id_is_archived");

            builder.HasIndex(account => account.UserId)
                .HasDatabaseName("ix_accounts_user_id");
        }
    }
}

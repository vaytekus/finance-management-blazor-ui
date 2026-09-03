using FinanceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.Configuration;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{

    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(w => w.Currency)
            .HasConversion<string>()
            .HasMaxLength(3);

        builder.Property(w => w.CreatedAt)
            .IsRequired();
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(w => new
            {
                w.UserId, w.Name
            })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        builder.HasQueryFilter(w => w.DeletedAt == null);
    }
}

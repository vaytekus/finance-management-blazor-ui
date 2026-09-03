using FinanceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.Configuration;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.HasIndex(x => x.UserId);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.ReplacedByToken)
            .HasMaxLength(256);

        builder.Property(x => x.ReasonRevoked)
            .HasMaxLength(100);
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Ignore(x => x.IsExpired);
        builder.Ignore(x => x.IsRevoked);
        builder.Ignore(x => x.IsActive);
    }
}

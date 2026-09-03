using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Id)
            .ValueGeneratedNever();
        
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.HasData(
            new Role { Id = UserRole.User, Name = UserRole.User.ToString() },
            new Role { Id = UserRole.Admin, Name = UserRole.Admin.ToString() }
        );
    }
}

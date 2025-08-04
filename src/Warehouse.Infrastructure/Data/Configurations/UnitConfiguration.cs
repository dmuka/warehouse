using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Infrastructure.Data.Configurations;

internal class UnitConfiguration : IEntityTypeConfiguration<UnitDto>
{
    public void Configure(EntityTypeBuilder<UnitDto> builder)
    {
        builder.ToTable("Units");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.HasIndex(r => r.UnitName)
            .IsUnique()
            .HasFilter("IsActive = 1");
    }
}
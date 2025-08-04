using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Infrastructure.Data.Configurations;

internal class BalanceConfiguration : IEntityTypeConfiguration<BalanceDto>
{
    public void Configure(EntityTypeBuilder<BalanceDto> builder)
    {
        builder.ToTable("Balances");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.UnitId)
            .IsRequired();
        
        builder.Property(r => r.ResourceId)
            .IsRequired();
        
        builder.HasOne(b => b.ResourceDto);
            
        builder.HasOne(b => b.UnitDto);
        
        builder.Property(b => b.Quantity)
            .HasPrecision(8, 2);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Infrastructure.Data.Configurations;

internal class BalanceConfiguration : IEntityTypeConfiguration<Balance>
{
    public void Configure(EntityTypeBuilder<Balance> builder)
    {
        builder.ToTable("Balances");
        
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasConversion(id => id.Value, value => new BalanceId(value));
        
        builder.Property(r => r.UnitId)
            .IsRequired()
            .HasConversion(id => id.Value, value => new UnitId(value));
    
        builder.HasOne<Unit>() 
            .WithMany()            
            .HasForeignKey(u => u.UnitId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(r => r.ResourceId)
            .IsRequired()
            .HasConversion(id => id.Value, value => new ResourceId(value));
    
        builder.HasOne<Resource>() 
            .WithMany()            
            .HasForeignKey(r => r.ResourceId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(b => b.Quantity)
            .HasPrecision(8, 2);
    }
}
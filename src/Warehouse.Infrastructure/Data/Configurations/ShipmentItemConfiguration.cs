using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Aggregates.Shipments;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Infrastructure.Data.Configurations;

internal class ShipmentItemConfiguration : IEntityTypeConfiguration<ShipmentItem>
{
    public void Configure(EntityTypeBuilder<ShipmentItem> builder)
    {
        builder.ToTable("ShipmentItems");
        
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id)
            .HasConversion(id => id.Value, value => new ShipmentItemId(value));
        
        builder.Property(i => i.ShipmentId)
            .IsRequired()
            .HasConversion(id => id.Value, value => new ShipmentId(value));
        
        builder.Property(i => i.ResourceId)
            .IsRequired()
            .HasConversion(id => id.Value, value => new ResourceId(value));
    
        builder.HasOne<Resource>() 
            .WithMany()            
            .HasForeignKey(i => i.ResourceId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(i => i.UnitId)
            .IsRequired()
            .HasConversion(id => id.Value, value => new UnitId(value));
    
        builder.HasOne<Unit>() 
            .WithMany()            
            .HasForeignKey(i => i.UnitId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(item => item.Quantity)
            .HasPrecision(8, 2);
    }
}
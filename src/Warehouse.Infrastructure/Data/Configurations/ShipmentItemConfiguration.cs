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
            .HasConversion(id => id.Value, value => new ShipmentId(value));
        
        builder.Property(item => item.ShipmentId)
            .HasConversion(id => id.Value, value => new ShipmentId(value));
        
        builder.Property(item => item.ResourceId)
            .HasConversion(id => id.Value, value => new ResourceId(value));
        
        builder.Property(item => item.UnitId)
            .HasConversion(id => id.Value, value => new UnitId(value));
        
        builder.Property(item => item.Quantity)
            .HasPrecision(8, 2);
    }
}
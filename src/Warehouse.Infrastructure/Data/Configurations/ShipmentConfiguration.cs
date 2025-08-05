using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Infrastructure.Data.Configurations;

internal class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");
        
        builder.HasKey(shipment => shipment.Id);
        builder.Property(shipment => shipment.Id)
            .HasConversion(id => id.Value, value => new ShipmentId(value));
        
        builder.HasMany(shipment => shipment.Items)
            .WithOne()
            .HasForeignKey(item => item.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
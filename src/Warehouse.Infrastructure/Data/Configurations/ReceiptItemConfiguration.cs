using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Aggregates.Receipts;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Infrastructure.Data.Configurations;

internal class ReceiptItemConfiguration : IEntityTypeConfiguration<ReceiptItem>
{
    public void Configure(EntityTypeBuilder<ReceiptItem> builder)
    {
        builder.ToTable("ReceiptItems");
        
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id)
            .HasConversion(id => id.Value, value => new ReceiptId(value));
        
        builder.Property(item => item.ReceiptId)
            .HasConversion(id => id.Value, value => new ReceiptId(value));
        
        builder.Property(item => item.ResourceId)
            .HasConversion(id => id.Value, value => new ResourceId(value));
        
        builder.Property(item => item.UnitId)
            .HasConversion(id => id.Value, value => new UnitId(value));
        
        builder.Property(item => item.Quantity)
            .HasPrecision(8, 2);
    }
}
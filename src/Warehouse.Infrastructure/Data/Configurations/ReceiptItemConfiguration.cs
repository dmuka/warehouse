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
            .HasConversion(id => id.Value, value => new ReceiptItemId(value));
        
        builder.Property(p => p.ReceiptId)
            .IsRequired()
            .HasConversion(id => id.Value, value => new ReceiptId(value));
        
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
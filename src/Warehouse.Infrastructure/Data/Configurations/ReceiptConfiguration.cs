using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Infrastructure.Data.Configurations;

internal class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.ToTable("Receipts");
        
        builder.HasKey(receipt => receipt.Id);
        builder.Property(receipt => receipt.Id)
            .HasConversion(id => id.Value, value => new ReceiptId(value));
        
        builder.HasMany(receipt => receipt.Items)
            .WithOne()
            .HasForeignKey(item => item.ReceiptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
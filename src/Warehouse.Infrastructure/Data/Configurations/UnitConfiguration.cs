using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Aggregates.Shipments;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Infrastructure.Data.Configurations;

internal class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units");
        
        builder.HasKey(unit => unit.Id);
        builder.Property(unit => unit.Id)
            .HasConversion(id => id.Value, value => new UnitId(value));
        
        builder.Property(unit => unit.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        
        builder.ComplexProperty(resource => resource.UnitName, b =>
        {
            b.Property(name => name.Value).HasColumnName("UnitName");
        });
    }
}
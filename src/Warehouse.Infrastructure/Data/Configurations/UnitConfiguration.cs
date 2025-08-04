using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        
        builder.Property(unit => unit.UnitName)
            .HasConversion(name => name.Value, value => UnitName.Create(value).Value);
            
        builder.HasIndex(unit => unit.UnitName)
            .IsUnique()
            .HasFilter("IsActive = 1");
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Infrastructure.Data.Configurations;

internal class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("Resources");
        
        builder.HasKey(resource => resource.Id);
        builder.Property(resource => resource.Id)
            .HasConversion(id => id.Value, value => new ResourceId(value));
        
        builder.Property(resource => resource.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        
        builder.Property(resource => resource.ResourceName)
            .HasConversion(name => name.Value, value => ResourceName.Create(value).Value);
        
        builder.HasIndex(resource => resource.ResourceName)
            .IsUnique()
            .HasFilter("IsActive = 1");
    }
}
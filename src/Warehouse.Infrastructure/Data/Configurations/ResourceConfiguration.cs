using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Aggregates.Resources.Constants;

namespace Warehouse.Infrastructure.Data.Configurations;

internal class ResourceConfiguration : IEntityTypeConfiguration<Domain.Aggregates.Resources.Resource>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Resources.Resource> builder)
    {
        builder.ToTable("Resources");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.ResourceName)
            .IsRequired()
            .HasMaxLength(ResourceConstants.ResourceNameMaxLength);
            
        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.HasIndex(r => r.ResourceName)
            .IsUnique()
            .HasFilter("IsActive = 1");
    }
}
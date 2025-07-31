using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Resources.Constants;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Infrastructure.Data.Configurations;

internal class ResourceConfiguration : IEntityTypeConfiguration<ResourceDto>
{
    public void Configure(EntityTypeBuilder<ResourceDto> builder)
    {
        builder.ToTable("Resources");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.HasIndex(r => r.ResourceName)
            .IsUnique()
            .HasFilter("IsActive = 1");
    }
}
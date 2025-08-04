using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Infrastructure.Data.Configurations;

internal class ClientConfiguration : IEntityTypeConfiguration<ClientDto>
{
    public void Configure(EntityTypeBuilder<ClientDto> builder)
    {
        builder.ToTable("Clients");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.HasIndex(r => r.ClientName)
            .IsUnique()
            .HasFilter("IsActive = 1");
    }
}
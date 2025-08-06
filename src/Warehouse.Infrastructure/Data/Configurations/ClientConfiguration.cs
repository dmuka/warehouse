using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Infrastructure.Data.Configurations;

internal class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");
        
        builder.HasKey(client => client.Id);
        builder.Property(client => client.Id)
            .HasConversion(id => id.Value, value => new ClientId(value));
        
        builder.Property(client => client.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.ComplexProperty(client => client.ClientName);
        builder.ComplexProperty(client => client.ClientAddress);
    }
}
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.UnitTests.Domain.Aggregates.Clients;

[TestFixture]
public class ClientTests
{
    private const string ValidClientName = "ClientName";
    private const string ValidClientAddress = "ClientAddress";
    private const string EmptyClientName = "";
    private const string EmptyClientAddress = "";
    
    [Test]
    public void Create_ValidParameters_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Client.Create(ValidClientName, ValidClientAddress);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.ClientName.Value, Is.EqualTo(ValidClientName));
            Assert.That(result.Value.IsActive, Is.True);
        }
    }

    [Test]
    public void Create_InvalidClientName_ReturnsValidationFailure()
    {
        // Arrange & Act
        var result = Client.Create(EmptyClientName, ValidClientAddress);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.Not.Null);
        }
    }

    [Test]
    public void Create_InvalidClientAddress_ReturnsValidationFailure()
    {
        // Arrange & Act
        var result = Client.Create(ValidClientName, EmptyClientAddress);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.Not.Null);
        }
    }

    [Test]
    public void SetName_ValidName_ReturnsSuccess()
    {
        // Arrange & Act
        var client = Client.Create(ValidClientName, ValidClientAddress).Value;
        var result = client.SetName("NewName");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void SetName_InvalidName_ReturnsValidationFailure()
    {
        // Arrange & Act
        var client = Client.Create(ValidClientName, ValidClientAddress).Value;
        var result = client.SetName(EmptyClientName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.Not.Null);
        }
    }

    [Test]
    public void SetAddress_ValidAddress_ReturnsSuccess()
    {
        // Arrange & Act
        var client = Client.Create(ValidClientName, ValidClientAddress).Value;
        var result = client.SetAddress("NewAddressValue");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void SetName_InvalidAddress_ReturnsValidationFailure()
    {
        // Arrange & Act
        var client = Client.Create(ValidClientName, ValidClientAddress).Value;
        var result = client.SetAddress(EmptyClientAddress);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.Not.Null);
        }
    }

    [Test]
    public void Activate_SetsIsActiveToTrue()
    {
        // Arrange & Act
        var client = Client.Create(ValidClientName, ValidClientAddress, false).Value;
        client.Activate();

        // Assert
        Assert.That(client.IsActive, Is.True);
    }

    [Test]
    public void Deactivate_SetsIsActiveToFalse()
    {
        // Arrange & Act
        var client = Client.Create(ValidClientName, ValidClientAddress).Value;
        client.Deactivate();

        // Assert
        Assert.That(client.IsActive, Is.False);
    }
}
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.UnitTests.Domain.Aggregates.Resources;

[TestFixture]
public class ResourceTests
{
    private const string ValidResourceName = "ValidResourceName";
    private const string EmptyResourceName = "";
    
    [Test]
    public void Create_ValidParameters_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Resource.Create(ValidResourceName);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.ResourceName.Value, Is.EqualTo(ValidResourceName));
            Assert.That(result.Value.IsActive, Is.True);
        }
    }

    [Test]
    public void Create_InvalidResourceName_ReturnsValidationFailure()
    {
        // Arrange & Act
        var result = Resource.Create(EmptyResourceName);

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
        var resource = Resource.Create(ValidResourceName).Value;
        var result = resource.SetName("NewValidName");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void SetName_InvalidName_ReturnsValidationFailure()
    {
        // Arrange & Act
        var resource = Resource.Create(ValidResourceName).Value;
        var result = resource.SetName("");

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
        var resource = Resource.Create(ValidResourceName, false).Value;
        resource.Activate();

        // Assert
        Assert.That(resource.IsActive, Is.True);
    }

    [Test]
    public void Deactivate_SetsIsActiveToFalse()
    {
        // Arrange & Act
        var resource = Resource.Create(ValidResourceName).Value;
        resource.Deactivate();

        // Assert
        Assert.That(resource.IsActive, Is.False);
    }
}
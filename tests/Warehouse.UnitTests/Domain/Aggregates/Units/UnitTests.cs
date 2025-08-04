using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.UnitTests.Domain.Aggregates.Units;

[TestFixture]
public class UnitTests
{
    private const string ValidUnitName = "UnitName";
    private const string EmptyUnitName = "";
    
    [Test]
    public void Create_ValidParameters_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Unit.Create(ValidUnitName);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.UnitName.Value, Is.EqualTo(ValidUnitName));
            Assert.That(result.Value.IsActive, Is.True);
        }
    }

    [Test]
    public void Create_InvalidUnitName_ReturnsValidationFailure()
    {
        // Arrange & Act
        var result = Unit.Create(EmptyUnitName);

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
        var unit = Unit.Create(ValidUnitName).Value;
        var result = unit.SetName("NewName");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void SetName_InvalidName_ReturnsValidationFailure()
    {
        // Arrange & Act
        var unit = Unit.Create(ValidUnitName).Value;
        var result = unit.SetName("");

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
        var unit = Unit.Create(ValidUnitName, false).Value;
        unit.Activate();

        // Assert
        Assert.That(unit.IsActive, Is.True);
    }

    [Test]
    public void Deactivate_SetsIsActiveToFalse()
    {
        // Arrange & Act
        var unit = Unit.Create(ValidUnitName).Value;
        unit.Deactivate();

        // Assert
        Assert.That(unit.IsActive, Is.False);
    }
}
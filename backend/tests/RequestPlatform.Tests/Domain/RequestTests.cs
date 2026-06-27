using FluentAssertions;
using RequestPlatform.Domain.Entities;

namespace RequestPlatform.Tests.Domain;

public class RequestTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateRequest()
    {
        // Arrange
        var type = "Vacation";
        var dynamicData = "{\"startDate\":\"2024-01-15\",\"endDate\":\"2024-01-20\",\"reason\":\"Family trip\"}";

        // Act
        var request = new Request(type, dynamicData);

        // Assert
        request.Id.Should().NotBeEmpty();
        request.Type.Should().Be(type);
        request.Status.Should().Be("Pending");
        request.DynamicData.Should().Be(dynamicData);
        request.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("InvalidType")]
    public void Constructor_WithInvalidType_ShouldThrowArgumentException(string invalidType)
    {
        // Arrange
        var dynamicData = "{\"key\":\"value\"}";

        // Act
        var act = () => new Request(invalidType, dynamicData);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-json")]
    [InlineData("{invalid}")]
    public void Constructor_WithInvalidDynamicData_ShouldThrowArgumentException(string invalidData)
    {
        // Arrange
        var type = "Loan";

        // Act
        var act = () => new Request(type, invalidData);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ValidateStatus_WithValidStatus_ShouldNotThrow()
    {
        // Act
        var act = () => Request.ValidateStatus("Approved");

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("InvalidStatus")]
    public void ValidateStatus_WithInvalidStatus_ShouldThrowArgumentException(string invalidStatus)
    {
        // Act
        var act = () => Request.ValidateStatus(invalidStatus);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ValidateType_WithAllValidTypes_ShouldNotThrow()
    {
        // Act & Assert
        var actVacation = () => Request.ValidateType("Vacation");
        var actLoan = () => Request.ValidateType("Loan");
        var actPermission = () => Request.ValidateType("Permission");

        actVacation.Should().NotThrow();
        actLoan.Should().NotThrow();
        actPermission.Should().NotThrow();
    }

    [Fact]
    public void UpdateStatus_WithValidStatus_ShouldUpdateStatus()
    {
        // Arrange
        var request = new Request("Vacation", "{\"days\": 5}");

        // Act
        request.UpdateStatus("Approved");

        // Assert
        request.Status.Should().Be("Approved");
    }
}

using System.Text.Json;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RequestPlatform.API.Controllers;
using RequestPlatform.Application.DTOs;
using RequestPlatform.Application.Services;

namespace RequestPlatform.Tests.Controllers;

public class RequestsControllerTests
{
    private readonly Mock<IRequestService> _mockService;
    private readonly Mock<IValidator<CreateRequestDto>> _mockValidator;
    private readonly RequestsController _controller;

    public RequestsControllerTests()
    {
        _mockService = new Mock<IRequestService>();
        _mockValidator = new Mock<IValidator<CreateRequestDto>>();
        _controller = new RequestsController(_mockService.Object, _mockValidator.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithData()
    {
        // Arrange
        var requests = new List<RequestDto>
        {
            new() { Id = Guid.NewGuid(), Type = "Vacation", Status = "Pending", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Type = "Loan", Status = "Approved", CreatedAt = DateTime.UtcNow }
        };
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<RequestFilterDto>())).ReturnsAsync(requests);

        // Act
        var result = await _controller.GetAll(new RequestFilterDto());

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var data = okResult.Value.Should().BeAssignableTo<IEnumerable<RequestDto>>().Subject;
        data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_WithExistingId_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new RequestDto
        {
            Id = id,
            Type = "Vacation",
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
        _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(request);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var data = okResult.Value.Should().BeOfType<RequestDto>().Subject;
        data.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((RequestDto?)null);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var dto = new CreateRequestDto
        {
            Type = "Vacation",
            DynamicData = JsonSerializer.Deserialize<JsonElement>("{\"days\": 5}")
        };
        var createdDto = new RequestDto
        {
            Id = Guid.NewGuid(),
            Type = "Vacation",
            Status = "Pending",
            DynamicData = JsonSerializer.Deserialize<JsonElement>("{\"days\": 5}"),
            CreatedAt = DateTime.UtcNow
        };

        _mockValidator.Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(createdDto);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task Create_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateRequestDto { Type = "", DynamicData = default };
        var failures = new List<ValidationFailure>
        {
            new("Type", "Type is required.")
        };
        _mockValidator.Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Delete_WithExistingId_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}

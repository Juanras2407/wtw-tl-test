using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RequestPlatform.Application.DTOs;
using RequestPlatform.Application.Services;

namespace RequestPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly IRequestService _requestService;
    private readonly IValidator<CreateRequestDto> _validator;

    public RequestsController(IRequestService requestService, IValidator<CreateRequestDto> validator)
    {
        _requestService = requestService;
        _validator = validator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] RequestFilterDto filters)
    {
        var requests = await _requestService.GetAllAsync(filters);
        return Ok(requests);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var request = await _requestService.GetByIdAsync(id);
        if (request is null)
            return NotFound();

        return Ok(request);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRequestDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new
            {
                e.PropertyName,
                e.ErrorMessage
            }));
        }

        var created = await _requestService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _requestService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

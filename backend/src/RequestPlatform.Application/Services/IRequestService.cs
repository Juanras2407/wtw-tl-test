using RequestPlatform.Application.DTOs;

namespace RequestPlatform.Application.Services;

public interface IRequestService
{
    Task<RequestDto> CreateAsync(CreateRequestDto dto);
    Task<IEnumerable<RequestDto>> GetAllAsync(RequestFilterDto filters);
    Task<RequestDto?> GetByIdAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
}

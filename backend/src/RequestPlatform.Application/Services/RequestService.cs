using System.Text.Json;
using RequestPlatform.Application.DTOs;
using RequestPlatform.Application.Mapping;
using RequestPlatform.Domain.Entities;
using RequestPlatform.Domain.Interfaces;

namespace RequestPlatform.Application.Services;

public class RequestService : IRequestService
{
    private readonly IRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RequestService(IRequestRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestDto> CreateAsync(CreateRequestDto dto)
    {
        var dynamicDataJson = dto.DynamicData.GetRawText();
        var request = new Request(dto.Type, dynamicDataJson);

        await _repository.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        return request.ToDto();
    }

    public async Task<IEnumerable<RequestDto>> GetAllAsync(RequestFilterDto filters)
    {
        var requests = await _repository.GetAllAsync(
            type: filters.Type,
            status: filters.Status,
            dateFrom: filters.DateFrom,
            dateTo: filters.DateTo,
            employeeName: filters.EmployeeName);

        return requests.ToDtos();
    }

    public async Task<RequestDto?> GetByIdAsync(Guid id)
    {
        var request = await _repository.GetByIdAsync(id);
        return request?.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var request = await _repository.GetByIdAsync(id);
        if (request is null)
            return false;

        await _repository.DeleteAsync(request);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}

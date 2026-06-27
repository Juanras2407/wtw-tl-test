using RequestPlatform.Domain.Entities;

namespace RequestPlatform.Domain.Interfaces;

public interface IRequestRepository
{
    Task<Request?> GetByIdAsync(Guid id);
    Task<IEnumerable<Request>> GetAllAsync(string? type = null, string? status = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? employeeName = null);
    Task AddAsync(Request request);
    Task DeleteAsync(Request request);
}

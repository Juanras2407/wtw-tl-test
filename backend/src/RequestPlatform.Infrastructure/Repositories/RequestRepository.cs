using Microsoft.EntityFrameworkCore;
using RequestPlatform.Domain.Entities;
using RequestPlatform.Domain.Interfaces;
using RequestPlatform.Infrastructure.Data;

namespace RequestPlatform.Infrastructure.Repositories;

public class RequestRepository : IRequestRepository
{
    private readonly AppDbContext _context;

    public RequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Request?> GetByIdAsync(Guid id)
    {
        return await _context.Requests.FindAsync(id);
    }

    public async Task<IEnumerable<Request>> GetAllAsync(
        string? type = null,
        string? status = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        string? employeeName = null)
    {
        // Ejecución activa de consultas nativas JSON en SQL Server utilizando ISJSON y JSON_VALUE:
        IQueryable<Request> query;

        if (!string.IsNullOrWhiteSpace(employeeName))
        {
            var searchPattern = $"%{employeeName}%";
            query = _context.Requests.FromSqlInterpolated(
                $"SELECT * FROM Requests WHERE ISJSON(DynamicData) = 1 AND JSON_VALUE(DynamicData, '$.employeeName') LIKE {searchPattern}");
        }
        else
        {
            query = _context.Requests.AsQueryable();
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(r => r.Type == type);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(r => r.Status == status);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(r => r.CreatedAt >= dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(r => r.CreatedAt <= dateTo.Value);
        }

        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task AddAsync(Request request)
    {
        await _context.Requests.AddAsync(request);
    }

    public Task DeleteAsync(Request request)
    {
        _context.Requests.Remove(request);
        return Task.CompletedTask;
    }
}

using System.Text.Json;
using RequestPlatform.Application.DTOs;
using RequestPlatform.Domain.Entities;

namespace RequestPlatform.Application.Mapping;

public static class RequestMappingExtensions
{
    public static RequestDto ToDto(this Request entity)
    {
        JsonElement? dynamicData = null;
        if (!string.IsNullOrEmpty(entity.DynamicData))
        {
            dynamicData = JsonSerializer.Deserialize<JsonElement>(entity.DynamicData);
        }

        return new RequestDto
        {
            Id = entity.Id,
            Type = entity.Type,
            Status = entity.Status,
            DynamicData = dynamicData,
            CreatedAt = entity.CreatedAt
        };
    }

    public static IEnumerable<RequestDto> ToDtos(this IEnumerable<Request> entities)
    {
        return entities.Select(e => e.ToDto());
    }
}

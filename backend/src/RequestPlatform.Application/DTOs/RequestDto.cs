using System.Text.Json;

namespace RequestPlatform.Application.DTOs;

public class RequestDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public JsonElement? DynamicData { get; set; }
    public DateTime CreatedAt { get; set; }
}

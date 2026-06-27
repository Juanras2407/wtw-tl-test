using System.Text.Json;

namespace RequestPlatform.Application.DTOs;

public class CreateRequestDto
{
    public string Type { get; set; } = string.Empty;
    public JsonElement DynamicData { get; set; }
}

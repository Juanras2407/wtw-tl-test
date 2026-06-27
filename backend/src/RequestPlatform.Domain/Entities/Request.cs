using System.Text.Json;
using RequestPlatform.Domain.Enums;

namespace RequestPlatform.Domain.Entities;

public class Request
{
    public Guid Id { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string DynamicData { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Request() { } // EF Core constructor

    public Request(string type, string dynamicData)
    {
        Id = Guid.NewGuid();
        ValidateType(type);
        Type = type;
        Status = RequestStatus.Pending.ToString();
        ValidateDynamicData(dynamicData);
        DynamicData = dynamicData;
        CreatedAt = DateTime.UtcNow;
    }

    public static void ValidateType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type is required.", nameof(type));

        if (!Enum.TryParse<RequestType>(type, ignoreCase: true, out _))
            throw new ArgumentException(
                $"Invalid request type: '{type}'. Valid types are: {string.Join(", ", Enum.GetNames<RequestType>())}.",
                nameof(type));
    }

    public static void ValidateStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status is required.", nameof(status));

        if (!Enum.TryParse<RequestStatus>(status, ignoreCase: true, out _))
            throw new ArgumentException(
                $"Invalid request status: '{status}'. Valid statuses are: {string.Join(", ", Enum.GetNames<RequestStatus>())}.",
                nameof(status));
    }

    public static void ValidateDynamicData(string dynamicData)
    {
        if (string.IsNullOrWhiteSpace(dynamicData))
            throw new ArgumentException("DynamicData is required.", nameof(dynamicData));

        try
        {
            using var doc = JsonDocument.Parse(dynamicData);
        }
        catch (JsonException)
        {
            throw new ArgumentException("DynamicData must be valid JSON.", nameof(dynamicData));
        }
    }

    public void UpdateStatus(string newStatus)
    {
        ValidateStatus(newStatus);
        Status = newStatus;
    }
}

namespace RequestPlatform.Application.DTOs;

public class RequestFilterDto
{
    public string? Type { get; set; }
    public string? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? EmployeeName { get; set; }
}
